namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.Enums;

public record VehicleTypeChangedDomainEvent(Guid Id, EVehicleType OldVehicleType, EVehicleType NewVehicleType) : IDomainEvent;
