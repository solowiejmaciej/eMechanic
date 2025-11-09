namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;

public record VehicleCreatedDomainEvent(Vehicle Vehicle) : IDomainEvent
{

}
