namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;

public record VehicleOwnerChangedDomainEvent(Guid Id, Guid OldOwnerId, Guid NewOwnerUserId) : IDomainEvent;
