namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using eMechanic.Application.Vehicle.Vehicle.Repositories;

public class VehicleHorsePowerChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleHorsePowerChangedDomainEvent>
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
