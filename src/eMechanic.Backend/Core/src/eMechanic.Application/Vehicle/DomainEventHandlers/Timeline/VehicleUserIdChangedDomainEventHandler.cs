namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;

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
            nameof(VehicleUserIdChangedDomainEventHandler),
            payload,
            cancellationToken);
    }
}
