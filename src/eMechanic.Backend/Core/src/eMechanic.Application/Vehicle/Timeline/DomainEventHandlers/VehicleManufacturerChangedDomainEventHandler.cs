namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using eMechanic.Application.Vehicle.Vehicle.Repositories;

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
