namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using eMechanic.Application.Vehicle.Vehicle.Repositories;

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
            nameof(VehicleTypeChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
