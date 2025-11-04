namespace eMechanic.OutboxPublisher;

using DAL;
using Dapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Npgsql;

public class OutboxPublisherFunction
{
    private readonly ILogger<OutboxPublisherFunction> _logger;
    private readonly NpgsqlDataSource _dataSource;

    public OutboxPublisherFunction(ILoggerFactory loggerFactory, NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
        _logger = loggerFactory.CreateLogger<OutboxPublisherFunction>();
    }

    [Function(nameof(OutboxPublisherFunction))]
    public async Task Run([TimerTrigger("* * * * *")] TimerInfo timerInfo, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Testowa funkcja 'Hello' została wywołana.");

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

        foreach (var message in messages)
        {
            _logger.LogInformation("Publishing message of type {MessageEventType} with payload {MessagePayload}", message.EventType, message.Payload);
        }
    }
}
