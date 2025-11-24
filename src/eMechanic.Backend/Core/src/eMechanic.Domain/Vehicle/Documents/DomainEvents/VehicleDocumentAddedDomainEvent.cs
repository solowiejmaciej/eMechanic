namespace eMechanic.Domain.Vehicle.Documents.DomainEvents;

using eMechanic.Common.DDD;

public record VehicleDocumentAddedDomainEvent(VehicleDocument Document) : IDomainEvent;
