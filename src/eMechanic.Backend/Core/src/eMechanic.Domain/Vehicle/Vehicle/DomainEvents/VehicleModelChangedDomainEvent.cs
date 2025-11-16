namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleModelChangedDomainEvent(Guid Id, Model OldModel, Model Model) : IDomainEvent;
