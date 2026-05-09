namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;

public record VehicleCreatedDomainEvent(Vehicle Vehicle) : IDomainEvent
{

}
