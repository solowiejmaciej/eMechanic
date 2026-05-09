namespace eMechanic.Application.Abstractions.Outbox;

using Events.Events;

public interface IOutboxWriter
{
    Task WriteAsync(IEvent @event, CancellationToken cancellationToken);
}
