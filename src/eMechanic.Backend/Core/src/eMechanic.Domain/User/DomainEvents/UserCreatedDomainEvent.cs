namespace eMechanic.Domain.User.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.User;

public record UserCreatedDomainEvent(User User) : IDomainEvent
{

}
