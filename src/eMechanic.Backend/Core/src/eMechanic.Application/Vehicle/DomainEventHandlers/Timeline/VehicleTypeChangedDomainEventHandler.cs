namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using Repostories;

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
