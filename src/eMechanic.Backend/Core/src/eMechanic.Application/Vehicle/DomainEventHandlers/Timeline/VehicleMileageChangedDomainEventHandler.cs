namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using Repostories;

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
