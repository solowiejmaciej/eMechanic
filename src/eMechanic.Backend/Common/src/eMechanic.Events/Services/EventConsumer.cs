namespace eMechanic.Events.Services;

using eMechanic.Events.Events;
using MassTransit;

public interface IEventConsumer<TEvent> : IConsumer<TEvent> where TEvent : class, IEvent
{
}
