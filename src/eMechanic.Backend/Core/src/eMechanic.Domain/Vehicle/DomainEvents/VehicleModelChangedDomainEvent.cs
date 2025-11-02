namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleModelChangedDomainEvent(Guid Id, Model OldModel, Model Model) : IDomainEvent;
