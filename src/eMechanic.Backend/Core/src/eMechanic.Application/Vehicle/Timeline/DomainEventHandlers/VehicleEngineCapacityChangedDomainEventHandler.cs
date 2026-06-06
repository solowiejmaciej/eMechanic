namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using eMechanic.Application.Vehicle.Vehicle.Repositories;

public class VehicleEngineCapacityChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleEngineCapacityChangedDomainEvent>
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
