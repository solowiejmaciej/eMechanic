namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using ValueObjects;

public record VehicleMileageChangedDomainEvent(Guid Id, Mileage OldMileage, Mileage Mileage) : IDomainEvent;
