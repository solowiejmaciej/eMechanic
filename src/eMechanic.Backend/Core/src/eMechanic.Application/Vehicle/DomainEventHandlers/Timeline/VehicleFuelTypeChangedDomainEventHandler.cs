namespace eMechanic.Application.Vehicle.DomainEventHandlers;

using Abstractions.DomainEvents;
using Abstractions.VehicleTimeline;
using Application.Timeline;
using Domain.Vehicle.DomainEvents;
using Microsoft.Extensions.Logging;
using Timeline;

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
