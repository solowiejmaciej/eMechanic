namespace eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;

using Domain.Vehicle;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using Microsoft.Extensions.Logging;
using Repostories;

public class VehicleCreatedDomainEventHandler : BaseTimelineEventHandler, IDomainEventHandler<VehicleCreatedDomainEvent>
{
    public VehicleCreatedDomainEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository) : base(vehicleVehicleTimelineRepository)
    {
    }

    public Task Handle(VehicleCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var vehicle = notification.Vehicle;

        var payload = new
        {
            UserId = vehicle.UserId,
            Vin = vehicle.Vin.Value,
            Manufacturer = vehicle.Manufacturer.Value,
            Model = vehicle.Model.Value,
            ProductionYear = vehicle.ProductionYear.Value,
            EngineCapacity = vehicle.EngineCapacity?.Value,
            Mileage = vehicle.Mileage.Value,
            MileageUnit = vehicle.Mileage.Unit,
            FuelType = vehicle.FuelType,
            BodyType = vehicle.BodyType,
            VehicleType = vehicle.VehicleType,
        };

        return CreateTimelineEntryAsync(
            vehicle.Id,
            nameof(VehicleCreatedDomainEvent),
            payload,
            cancellationToken);
    }
}
