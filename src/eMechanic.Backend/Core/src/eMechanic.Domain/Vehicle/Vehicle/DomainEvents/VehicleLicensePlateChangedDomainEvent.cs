namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using ValueObjects;

public record VehicleLicensePlateChangedDomainEvent(Guid Id, LicensePlate OldLicensePlate, LicensePlate LicensePlate) : IDomainEvent;
