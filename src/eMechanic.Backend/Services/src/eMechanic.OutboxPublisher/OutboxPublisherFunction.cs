namespace eMechanic.OutboxPublisher;

using DAL;
using Dapper;
using Events.Factories;
using Events.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Npgsql;

public class OutboxPublisherFunction
{
    private readonly ILogger<OutboxPublisherFunction> _logger;
    private readonly NpgsqlDataSource _dataSource;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventFactory _eventFactory;

    public OutboxPublisherFunction(
        ILoggerFactory loggerFactory,
        NpgsqlDataSource dataSource,
        IEventPublisher eventPublisher,
        IEventFactory eventFactory)
    {
        _dataSource = dataSource;
        _eventPublisher = eventPublisher;
        _eventFactory = eventFactory;
        _logger = loggerFactory.CreateLogger<OutboxPublisherFunction>();
    }

    [Function(nameof(OutboxPublisherFunction))]
    public async Task Run([TimerTrigger("* * * * *")] TimerInfo timerInfo, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Function invoked");

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        const string querySql = $"""
                                 SELECT "Id", "Payload", "EventType"
                                 FROM "OutboxMessages"
                                 WHERE "ProcessedAt" IS NULL
                                 ORDER BY "CreatedAt"
                                 LIMIT 20
                                 FOR UPDATE SKIP LOCKED
                                 """;

        List<OutboxMessageDto> messages;
        try
        {
            messages = (await connection.QueryAsync<OutboxMessageDto>(querySql, transaction: transaction)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query OutboxMessages.");
            await transaction.RollbackAsync(cancellationToken);
            return;
        }

        if (messages.Count == 0)
        {
            _logger.LogInformation("No new messages to publish");
            return;
        }

        foreach (var message in messages)
        {
            var result = _eventFactory.Create(message.EventType, message.Payload);
            if (result is null)
            {
                _logger.LogWarning("Event factory result for type {MessageEventType} was null.", message.EventType);
                continue;
            }

            if (result.Event is null || result.Type is null)
            {
                _logger.LogWarning("Event of type {MessageEventType} was null.", message.EventType);
                continue;
            }

            var @event = result.Event;
            var type = result.Type;

            _logger.LogInformation("Publishing message of type {MessageEventType} with payload {MessagePayload}", @event.GetType().Name, message.Payload);

            await _eventPublisher.PublishAsync(@event, type, cancellationToken);
        }
    }
}
