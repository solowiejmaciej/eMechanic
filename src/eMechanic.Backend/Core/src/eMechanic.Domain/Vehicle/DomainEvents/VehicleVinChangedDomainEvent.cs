namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleVinChangedDomainEvent(Guid Id, Vin OldVin, Vin Vin) : IDomainEvent;
