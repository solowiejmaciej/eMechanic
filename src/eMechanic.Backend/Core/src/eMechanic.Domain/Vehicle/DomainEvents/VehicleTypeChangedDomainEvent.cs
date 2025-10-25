namespace eMechanic.Domain.Vehicle;

using Common.DDD;
using Enums;

public record VehicleTypeChangedDomainEvent(Guid Id, EVehicleType OldVehicleType, EVehicleType NewVehicleType) : IDomainEvent;
