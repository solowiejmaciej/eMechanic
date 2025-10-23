namespace eMechanic.Application.Abstractions.DomainEvents;

using Common.DDD;
using MediatR;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent;
