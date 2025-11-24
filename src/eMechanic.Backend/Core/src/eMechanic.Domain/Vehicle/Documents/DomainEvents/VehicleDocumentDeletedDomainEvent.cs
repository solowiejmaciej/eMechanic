namespace eMechanic.Domain.Vehicle.Documents.DomainEvents;

using eMechanic.Common.DDD;

public record VehicleDocumentDeletedDomainEvent(VehicleDocument Document) : IDomainEvent;
