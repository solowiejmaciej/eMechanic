namespace eMechanic.Domain.Vehicle;

using Common.DDD;
using ValueObjects;

public record VehicleProductionYearChangedDomainEvent(Guid Id, ProductionYear OldProductionYear, ProductionYear ProductionYear) : IDomainEvent;
