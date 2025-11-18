namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;

public record RepairRequestRejectedDomainEvent(RepairRequest RepairRequest) : IDomainEvent;
