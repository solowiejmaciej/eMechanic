namespace eMechanic.Application.Vehicle.DomainEventHandlers;

using Abstractions.DomainEvents;
using Abstractions.VehicleTimeline;
using Application.Timeline;
using Domain.Vehicle;
using Domain.Vehicle.DomainEvents;
using Microsoft.Extensions.Logging;
using Timeline;

public class VehicleTypeChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleTypeChangedDomainEvent>
{
    public VehicleTypeChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleTypeChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldVehicleType;
        var newValue = notification.NewVehicleType;

        var payload = new
        {
            VehicleType = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleTypeChangedDomainEventHandler),
            payload,
            cancellationToken);
    }
}
