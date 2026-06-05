namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using eMechanic.Application.Vehicle.Vehicle.Repositories;

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
