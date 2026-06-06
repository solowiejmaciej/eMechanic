namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using eMechanic.Application.Vehicle.Vehicle.Repositories;

public class VehicleBodyTypeChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleBodyTypeChangedDomainEvent>
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
