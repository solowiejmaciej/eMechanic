namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Timeline;

public class VehicleUserIdChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleUserIdChangedDomainEvent>
{
    public VehicleUserIdChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleUserIdChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldOwnerId;
        var newValue = notification.NewOwnerUserId;

        var payload = new
        {
            UserId = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleUserIdChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
