namespace eMechanic.Domain.Vehicle.Documents.DomainEvents;

using eMechanic.Common.DDD;
using VehicleDocument;

public record VehicleDocumentDeletedDomainEvent(VehicleDocument Document) : IDomainEvent;
