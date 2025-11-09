namespace eMechanic.Domain.Vehicle;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.Enums;

public record VehicleBodyTypeChangedDomainEvent(Guid Id, EBodyType OldBodyType, EBodyType NewBodyType) : IDomainEvent;
