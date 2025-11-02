namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;

public class VehicleVinChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleVinChangedDomainEvent>
{
    public VehicleVinChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleVinChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldVin;
        var newValue = notification.Vin;

        var payload = new
        {
            Vin = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleVinChangedDomainEventHandler),
            payload,
            cancellationToken);
    }
}
