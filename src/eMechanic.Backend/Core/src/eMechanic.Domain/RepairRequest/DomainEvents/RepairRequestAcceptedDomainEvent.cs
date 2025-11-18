namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;

public record RepairRequestAcceptedDomainEvent(RepairRequest RepairRequest) : IDomainEvent;
