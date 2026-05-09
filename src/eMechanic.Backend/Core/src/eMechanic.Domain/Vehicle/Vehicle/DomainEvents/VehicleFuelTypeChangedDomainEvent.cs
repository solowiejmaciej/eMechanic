namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using Enums;

public record VehicleFuelTypeChangedDomainEvent(Guid Id, EFuelType OldFuelType, EFuelType NewFuelType) : IDomainEvent;
