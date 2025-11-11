namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Domain.Vehicle;
using Microsoft.Extensions.Logging;
using Repostories;

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
