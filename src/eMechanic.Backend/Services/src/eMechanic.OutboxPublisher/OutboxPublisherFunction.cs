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
    private const int BATCH_SIZE = 20;

    private const string QUERY_SQL = """
        SELECT "Id", "Payload", "EventType"
        FROM "OutboxMessages"
        WHERE "ProcessedAt" IS NULL
        ORDER BY "CreatedAt"
        LIMIT @BatchSize
        FOR UPDATE SKIP LOCKED
        """;

    private const string UPDATE_SQL = """
        UPDATE "OutboxMessages"
        SET "ProcessedAt" = @ProcessedAt
        WHERE "Id" = ANY(@Ids)
        """;

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
        _logger.LogInformation("Outbox publisher invoked");

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        List<OutboxMessageDto> messages;
        try
        {
            messages = (await connection.QueryAsync<OutboxMessageDto>(QUERY_SQL, new { BatchSize = BATCH_SIZE }, transaction: transaction)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query OutboxMessages");
            await transaction.RollbackAsync(cancellationToken);
            return;
        }

        if (messages.Count == 0)
        {
            _logger.LogDebug("No pending messages in outbox");
            await transaction.RollbackAsync(cancellationToken);
            return;
        }

        _logger.LogInformation("Processing {Count} outbox message(s)", messages.Count);

        var processedIds = new List<Guid>(messages.Count);
        var failedCount = 0;

        foreach (var message in messages)
        {
            try
            {
                var result = _eventFactory.Create(message.EventType, message.Payload);

                if (result?.Event is null || result.Type is null)
                {
                    _logger.LogWarning(
                        "Skipping message {Id}: could not create event of type {EventType}",
                        message.Id, message.EventType);
                    failedCount++;
                    continue;
                }

                await _eventPublisher.PublishAsync(result.Event, result.Type, cancellationToken);
                processedIds.Add(message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to publish message {Id} of type {EventType}",
                    message.Id, message.EventType);
                failedCount++;
            }
        }

        if (processedIds.Count > 0)
        {
            await connection.ExecuteAsync(
                UPDATE_SQL,
                new { ProcessedAt = DateTime.UtcNow, Ids = processedIds.ToArray() },
                transaction: transaction);
        }

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Outbox run complete — published: {Published}, skipped/failed: {Failed}",
            processedIds.Count, failedCount);
    }
}
