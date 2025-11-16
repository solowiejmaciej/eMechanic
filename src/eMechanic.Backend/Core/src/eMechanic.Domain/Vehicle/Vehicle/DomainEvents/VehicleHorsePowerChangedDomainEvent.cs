namespace eMechanic.Domain.Vehicle;

using Common.DDD;
using ValueObjects;

public record VehicleHorsePowerChangedDomainEvent(Guid Id, HorsePower OldHorsePower, HorsePower HorsePower) : IDomainEvent;
