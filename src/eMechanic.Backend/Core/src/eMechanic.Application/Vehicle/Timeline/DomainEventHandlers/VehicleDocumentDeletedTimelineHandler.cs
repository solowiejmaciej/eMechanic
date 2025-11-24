namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle.Documents.DomainEvents;
using Vehicle.Repostories;

internal sealed class VehicleDocumentDeletedTimelineHandler
    : BaseTimelineEventHandler, IDomainEventHandler<VehicleDocumentDeletedDomainEvent>
{
    public VehicleDocumentDeletedTimelineHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository)
        : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleDocumentDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var doc = notification.Document;

        var payload = new
        {
            DocumentId = doc.Id,
            FileName = doc.OriginalFileName,
            DocumentType = doc.DocumentType.ToString()
        };

        return CreateTimelineEntryAsync(
            doc.VehicleId,
            nameof(VehicleDocumentDeletedDomainEvent),
            payload,
            cancellationToken);
    }
}
