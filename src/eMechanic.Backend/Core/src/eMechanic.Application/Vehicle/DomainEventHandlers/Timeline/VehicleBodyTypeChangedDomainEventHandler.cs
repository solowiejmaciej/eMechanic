namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using Repostories;

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
