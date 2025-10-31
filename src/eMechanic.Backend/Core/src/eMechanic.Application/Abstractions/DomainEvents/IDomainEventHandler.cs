namespace eMechanic.Application.Abstractions.DomainEvents;

using Common.DDD;
using MediatR;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent;
