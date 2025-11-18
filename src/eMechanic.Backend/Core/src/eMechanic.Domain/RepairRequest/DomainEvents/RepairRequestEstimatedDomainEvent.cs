namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;

public record RepairRequestEstimatedDomainEvent(RepairRequest RepairRequest) : IDomainEvent;
