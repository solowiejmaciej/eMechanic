namespace eMechanic.Domain.Vehicle;

using Common.DDD;
using ValueObjects;

public record VehicleModelChangedDomainEvent(Guid Id, Model OldModel, Model Model) : IDomainEvent;
