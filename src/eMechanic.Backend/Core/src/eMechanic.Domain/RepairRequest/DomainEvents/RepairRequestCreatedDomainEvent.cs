namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;

public record RepairRequestCreatedDomainEvent(RepairRequest RepairRequest) : IDomainEvent;
