namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.ValueObjects;

public record VehicleProductionYearChangedDomainEvent(Guid Id, ProductionYear OldProductionYear, ProductionYear ProductionYear) : IDomainEvent;
