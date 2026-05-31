namespace eMechanic.Domain.Repair.DomainEvents;

using Common.DDD;

public sealed record RepairStartedDomainEvent(Repair Repair) : IDomainEvent;

