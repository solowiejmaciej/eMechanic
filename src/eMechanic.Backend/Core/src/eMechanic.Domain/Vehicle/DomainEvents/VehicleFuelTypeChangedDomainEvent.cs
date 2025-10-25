namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.Enums;

public record VehicleFuelTypeChangedDomainEvent(Guid Id, EFuelType OldFuelType, EFuelType NewFuelType) : IDomainEvent;
