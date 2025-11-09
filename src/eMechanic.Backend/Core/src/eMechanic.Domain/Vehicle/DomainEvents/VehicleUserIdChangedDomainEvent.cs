namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;

public record VehicleUserIdChangedDomainEvent(Guid Id, Guid OldOwnerId, Guid NewOwnerUserId) : IDomainEvent;
