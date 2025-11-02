namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle.DomainEvents;

public class VehicleMileageChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleMileageChangedDomainEvent>
{
    public VehicleMileageChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleMileageChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldMileage;
        var newValue = notification.Mileage;

        var payload = new
        {
            Mileage = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleMileageChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
