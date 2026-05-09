namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using ValueObjects;

public record VehicleProductionYearChangedDomainEvent(Guid Id, ProductionYear OldProductionYear, ProductionYear ProductionYear) : IDomainEvent;
