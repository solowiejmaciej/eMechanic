namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle.DomainEvents;
using Microsoft.Extensions.Logging;

public class VehicleBodyTypeChangedDomainEventHandler : BaseTimelineEventHandler , IDomainEventHandler<VehicleBodyTypeChangedDomainEvent>
{
    public VehicleBodyTypeChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleBodyTypeChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldBodyType;
        var newValue = notification.NewBodyType;

        var payload = new
        {
            BodyType = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleBodyTypeChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
