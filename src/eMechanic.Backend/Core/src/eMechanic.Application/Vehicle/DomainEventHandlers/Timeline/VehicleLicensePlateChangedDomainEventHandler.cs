namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Abstractions.DomainEvents;
using Application.Timeline;
using Domain.Vehicle;
using Repostories;

public class VehicleLicensePlateChangedDomainEventHandler : BaseTimelineEventHandler , IDomainEventHandler<VehicleLicensePlateChangedDomainEvent>
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
