namespace eMechanic.Domain.User.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.User;
using Events.Events;
using Events.Events.User;

public record UserCreatedDomainEvent(User User) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent() => new UserCreatedEvent(User.Email, User.FirstName, User.LastName, User.Id);
}
