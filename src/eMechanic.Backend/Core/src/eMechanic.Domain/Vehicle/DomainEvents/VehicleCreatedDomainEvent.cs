namespace eMechanic.Domain.Vehicle.DomainEvents;

using Common.DDD;

public record VehicleCreatedDomainEvent(Vehicle Vehicle) : IDomainEvent
{

}
