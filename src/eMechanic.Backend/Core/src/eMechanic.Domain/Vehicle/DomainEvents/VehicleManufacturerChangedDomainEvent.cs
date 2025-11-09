namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleManufacturerChangedDomainEvent(Guid Id, Manufacturer OldManufacturer, Manufacturer Manufacturer) : IDomainEvent;
