namespace eMechanic.Domain.Vehicle.Vehicle.DomainEvents;

using eMechanic.Common.DDD;
using Enums;

public record VehicleBodyTypeChangedDomainEvent(Guid Id, EBodyType OldBodyType, EBodyType NewBodyType) : IDomainEvent;
