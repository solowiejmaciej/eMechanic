namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using Enums;

public record VehicleTypeChangedDomainEvent(Guid Id, EVehicleType OldVehicleType, EVehicleType NewVehicleType) : IDomainEvent;
