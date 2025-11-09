namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleMileageChangedDomainEvent(Guid Id, Mileage OldMileage, Mileage Mileage) : IDomainEvent;
