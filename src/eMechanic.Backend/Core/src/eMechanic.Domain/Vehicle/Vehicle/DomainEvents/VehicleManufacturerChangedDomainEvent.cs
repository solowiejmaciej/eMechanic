namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using ValueObjects;

public record VehicleManufacturerChangedDomainEvent(Guid Id, Manufacturer OldManufacturer, Manufacturer Manufacturer) : IDomainEvent;
