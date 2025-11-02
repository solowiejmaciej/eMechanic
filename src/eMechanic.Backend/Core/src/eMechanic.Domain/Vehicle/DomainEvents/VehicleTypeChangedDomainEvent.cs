namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.Enums;

public record VehicleTypeChangedDomainEvent(Guid Id, EVehicleType OldVehicleType, EVehicleType NewVehicleType) : IDomainEvent;
