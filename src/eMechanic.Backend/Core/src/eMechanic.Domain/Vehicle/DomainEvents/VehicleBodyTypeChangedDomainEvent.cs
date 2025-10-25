namespace eMechanic.Domain.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using eMechanic.Domain.Vehicle.Enums;

public record VehicleBodyTypeChangedDomainEvent(Guid Id, EBodyType OldBodyType, EBodyType NewBodyType) : IDomainEvent;
