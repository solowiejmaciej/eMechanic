namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using Repostories;

public class VehicleManufacturerChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleManufacturerChangedDomainEvent>
{
    public VehicleManufacturerChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleManufacturerChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldManufacturer;
        var newValue = notification.Manufacturer;

        var payload = new
        {
            Manufacturer = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleManufacturerChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
