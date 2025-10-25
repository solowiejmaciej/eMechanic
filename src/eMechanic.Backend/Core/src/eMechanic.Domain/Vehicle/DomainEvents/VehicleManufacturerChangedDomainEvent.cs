namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleManufacturerChangedDomainEvent(Guid Id, Manufacturer OldManufacturer, Manufacturer Manufacturer) : IDomainEvent;
