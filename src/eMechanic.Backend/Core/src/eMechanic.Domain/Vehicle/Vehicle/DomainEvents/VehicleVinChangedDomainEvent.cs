namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using ValueObjects;

public record VehicleVinChangedDomainEvent(Guid Id, Vin OldVin, Vin Vin) : IDomainEvent;
