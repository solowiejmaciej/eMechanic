namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Application.Vehicle.Repostories;
using eMechanic.Domain.VehicleDocument.DomainEvents;

internal sealed class VehicleDocumentAddedTimelineHandler
    : BaseTimelineEventHandler, IDomainEventHandler<VehicleDocumentAddedDomainEvent>
{
    public VehicleDocumentAddedTimelineHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository)
        : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleDocumentAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var doc = notification.Document;

        var payload = new
        {
            DocumentId = doc.Id,
            FileName = doc.OriginalFileName,
            DocumentType = doc.DocumentType.ToString(),
            FullPath = doc.FullPath
        };

        return CreateTimelineEntryAsync(
            doc.VehicleId,
            nameof(VehicleDocumentAddedDomainEvent),
            payload,
            cancellationToken);
    }
}
