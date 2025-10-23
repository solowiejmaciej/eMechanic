namespace eMechanic.Domain.Workshop.DomainEvents;

using eMechanic.Common.DDD;

public record WorkshopCreatedDomainEvent(Workshop Workshop) : IDomainEvent
{

}
