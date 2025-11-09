namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Timeline;
using Microsoft.Extensions.Logging;

public class VehicleEngineCapacityChangedDomainEventHandler : BaseTimelineEventHandler , IDomainEventHandler<VehicleEngineCapacityChangedDomainEvent>
{
    public VehicleEngineCapacityChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleEngineCapacityChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldCapacity;
        var newValue = notification.NewCapacity;

        var payload = new
        {
            EngineCapacity = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleEngineCapacityChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
