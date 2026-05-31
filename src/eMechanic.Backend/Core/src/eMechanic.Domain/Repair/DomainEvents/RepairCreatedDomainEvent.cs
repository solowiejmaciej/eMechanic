namespace eMechanic.Domain.Repair.DomainEvents;

using Common.DDD;

public sealed record RepairCreatedDomainEvent(Repair Repair) : IDomainEvent;

