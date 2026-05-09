namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using ValueObjects;

public record VehicleHorsePowerChangedDomainEvent(Guid Id, HorsePower OldHorsePower, HorsePower HorsePower) : IDomainEvent;
