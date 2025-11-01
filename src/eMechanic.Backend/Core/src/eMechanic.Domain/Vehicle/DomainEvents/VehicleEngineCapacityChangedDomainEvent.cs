namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleEngineCapacityChangedDomainEvent(Guid Id, EngineCapacity? OldCapacity, EngineCapacity? NewCapacity) : IDomainEvent;
