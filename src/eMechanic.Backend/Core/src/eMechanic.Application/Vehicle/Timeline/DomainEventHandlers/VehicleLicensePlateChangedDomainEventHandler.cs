namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using Vehicle.Repostories;

public class VehicleLicensePlateChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleLicensePlateChangedDomainEvent>
{
    public VehicleLicensePlateChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleLicensePlateChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldLicensePlate;
        var newValue = notification.LicensePlate;

        var payload = new
        {
            LicensePlate = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleLicensePlateChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
