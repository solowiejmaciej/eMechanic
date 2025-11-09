namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Timeline;

public class VehicleFuelTypeChangedDomainEventHandler : BaseTimelineEventHandler , IDomainEventHandler<VehicleFuelTypeChangedDomainEvent>
{
    public VehicleFuelTypeChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleFuelTypeChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldFuelType;
        var newValue = notification.NewFuelType;

        var payload = new
        {
            FuelType = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleFuelTypeChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
