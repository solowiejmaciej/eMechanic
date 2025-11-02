namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;

public record VehicleUserIdChangedDomainEvent(Guid Id, Guid OldOwnerId, Guid NewOwnerUserId) : IDomainEvent;
