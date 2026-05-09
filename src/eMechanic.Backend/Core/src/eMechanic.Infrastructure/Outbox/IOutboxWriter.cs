namespace eMechanic.Infrastructure.Outbox;

using Application.Abstractions.Outbox;
using DAL;
using Events.Events;

internal sealed class OutboxWriter : IOutboxWriter
{
    private readonly AppDbContext _dbContext;

    public OutboxWriter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task WriteAsync(IEvent @event, CancellationToken cancellationToken)
    {
        var outboxMessage = new OutboxMessage(
            @event.GetType().Name,
            System.Text.Json.JsonSerializer.Serialize(@event, @event.GetType())
        );

        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }
}
