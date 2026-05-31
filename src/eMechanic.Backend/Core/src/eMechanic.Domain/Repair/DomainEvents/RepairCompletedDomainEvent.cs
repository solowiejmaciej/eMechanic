namespace eMechanic.Domain.Repair.DomainEvents;

using Common.DDD;

public sealed record RepairCompletedDomainEvent(Repair Repair) : IDomainEvent;

