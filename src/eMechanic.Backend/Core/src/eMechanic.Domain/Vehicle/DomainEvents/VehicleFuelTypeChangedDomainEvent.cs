namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.Enums;

public record VehicleFuelTypeChangedDomainEvent(Guid Id, EFuelType OldFuelType, EFuelType NewFuelType) : IDomainEvent;
