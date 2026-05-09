namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;
using Events.Events;
using Events.Events.RepairRequest;

public record RepairRequestCreatedDomainEvent(RepairRequest RepairRequest) : IDomainEvent;
