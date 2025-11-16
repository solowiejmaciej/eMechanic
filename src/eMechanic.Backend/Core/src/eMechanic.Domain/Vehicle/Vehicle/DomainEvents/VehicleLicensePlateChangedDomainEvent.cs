namespace eMechanic.Domain.Vehicle;

using Common.DDD;
using ValueObjects;

public record VehicleLicensePlateChangedDomainEvent(Guid Id, LicensePlate OldLicensePlate, LicensePlate LicensePlate) : IDomainEvent;
