namespace eMechanic.Domain.Repair.DomainEvents;

using Common.DDD;

public sealed record RepairPaidDomainEvent(Repair Repair) : IDomainEvent;

