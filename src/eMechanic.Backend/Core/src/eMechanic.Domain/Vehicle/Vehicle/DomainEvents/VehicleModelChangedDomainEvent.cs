namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using ValueObjects;

public record VehicleModelChangedDomainEvent(Guid Id, Model OldModel, Model Model) : IDomainEvent;
