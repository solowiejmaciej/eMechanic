namespace eMechanic.Domain.Users.DomainEvents;

using eMechanic.Common.DDD;

public record UserCreatedDomainEvent(User User) : IDomainEvent
{

}
