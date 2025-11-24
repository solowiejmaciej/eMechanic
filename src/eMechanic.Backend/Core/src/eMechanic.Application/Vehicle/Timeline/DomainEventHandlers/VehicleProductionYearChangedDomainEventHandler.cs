namespace eMechanic.Application.Vehicle.Timeline.DomainEventHandlers;

using Domain.Vehicle.Vehicle.DomainEvents;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using Vehicle.Repostories;

public class VehicleProductionYearChangedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleProductionYearChangedDomainEvent>
{
    public VehicleProductionYearChangedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleProductionYearChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var oldValue = notification.OldProductionYear;
        var newValue = notification.ProductionYear;

        var payload = new
        {
            ProductionYear = new
            {
                OldValue = oldValue,
                NewValue = newValue
            }
        };

        return CreateTimelineEntryAsync(
            notification.Id,
            nameof(VehicleProductionYearChangedDomainEvent),
            payload,
            cancellationToken);
    }
}
