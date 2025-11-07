namespace eMechanic.Events.Services;

using eMechanic.Events.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
        _logger.LogInformation("Event published: {Name}", @event.GetType().Name);
    }

    public async Task PublishAsync(IEvent @event, Type eventType, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(@event, eventType, cancellationToken);
        _logger.LogInformation("Event (from outbox) published: {Name}", eventType.Name);
    }
}

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
    Task PublishAsync(IEvent @event, Type eventType, CancellationToken cancellationToken);
}
