namespace eMechanic.Domain.Vehicle.DomainEvents;

using Common.DDD;
using ValueObjects;

public record VehicleMileageChangedDomainEvent(Guid Id, Mileage OldMileage, Mileage Mileage) : IDomainEvent;
