namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Abstractions.DomainEvents;
using Abstractions.VehicleTimeline;
using Application.Timeline;
using Domain.Vehicle;

public class VehicleHorsePowerChangedDomainEventHandler : BaseTimelineEventHandler , IDomainEventHandler<VehicleHorsePowerChangedDomainEvent>
{
    public VehicleHorsePowerChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleHorsePowerChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldHorsePower;
        var newValue = notification.HorsePower;

        var payload = new
        {
            HorsePower = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleHorsePowerChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
