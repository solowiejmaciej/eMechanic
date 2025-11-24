
namespace eMechanic.Application.RepairRequest.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Application.Vehicle.Vehicle.Repostories;
using eMechanic.Domain.RepairRequest.DomainEvents;

public class RepairRequestCreatedTimelineHandler : BaseTimelineEventHandler,
    IDomainEventHandler<RepairRequestCreatedDomainEvent>
{
    public RepairRequestCreatedTimelineHandler(IVehicleTimelineRepository vehicleTimelineRepository) : base(
        vehicleTimelineRepository)
    {
    }

    public Task Handle(RepairRequestCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repairRequest = notification.RepairRequest;

        var payload = new
        {
            WorkshopId = repairRequest.WorkshopId,
            Description = repairRequest.Description.Value,
            Status = repairRequest.Status.ToString()
        };

        return CreateTimelineEntryAsync(
            repairRequest.VehicleId,
            nameof(RepairRequestCreatedDomainEvent),
            payload,
            cancellationToken);
    }
}
