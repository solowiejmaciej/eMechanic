namespace eMechanic.Domain.VehicleDocument.DomainEvents;

using Common.DDD;

public record VehicleDocumentAddedDomainEvent(VehicleDocument Document) : IDomainEvent;
