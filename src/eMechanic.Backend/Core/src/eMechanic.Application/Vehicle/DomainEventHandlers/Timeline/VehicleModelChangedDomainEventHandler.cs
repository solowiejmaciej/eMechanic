namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using Repostories;

public class VehicleModelChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleModelChangedDomainEvent>
{
    public VehicleModelChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleModelChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldModel;
        var newValue = notification.Model;

        var payload = new
        {
            Model = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleModelChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
