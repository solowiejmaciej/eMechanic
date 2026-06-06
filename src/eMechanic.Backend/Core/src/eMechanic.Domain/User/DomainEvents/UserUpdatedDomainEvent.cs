namespace eMechanic.Domain.User.DomainEvents;

using eMechanic.Common.DDD;
using Events.Events;
using Events.Events.User;

public record UserUpdatedDomainEvent(User User) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent() => new UserUpdatedEvent(
        User.Email.Value, User.FirstName, User.LastName, User.Id, User.PhoneNumber?.Value);
}
