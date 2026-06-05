namespace eMechanic.Application.Repair.DomainEventHandlers.Timeline;

using Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Application.Vehicle.Vehicle.Repositories;
using eMechanic.Domain.Repair.DomainEvents;

public class RepairStartedTimelineHandler : BaseTimelineEventHandler,
    IDomainEventHandler<RepairStartedDomainEvent>
{
    public RepairStartedTimelineHandler(IVehicleTimelineRepository vehicleTimelineRepository) : base(
        vehicleTimelineRepository)
    {
    }

    public Task Handle(RepairStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repair = notification.Repair;

        var payload = new
        {
            WorkshopId = repair.WorkshopId,
            EstimatedCost = repair.EstimatedCost.Amount,
            Currency = repair.EstimatedCost.Currency,
            Status = repair.Status.ToString()
        };

        return CreateTimelineEntryAsync(
            repair.VehicleId,
            nameof(RepairStartedDomainEvent),
            payload,
            cancellationToken);
    }
}

