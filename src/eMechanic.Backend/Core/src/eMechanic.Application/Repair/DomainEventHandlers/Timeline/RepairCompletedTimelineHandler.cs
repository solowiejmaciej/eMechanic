namespace eMechanic.Application.Repair.DomainEventHandlers.Timeline;

using Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Application.Vehicle.Vehicle.Repositories;
using eMechanic.Domain.Repair.DomainEvents;

public class RepairCompletedTimelineHandler : BaseTimelineEventHandler,
    IDomainEventHandler<RepairCompletedDomainEvent>
{
    public RepairCompletedTimelineHandler(IVehicleTimelineRepository vehicleTimelineRepository) : base(
        vehicleTimelineRepository)
    {
    }

    public Task Handle(RepairCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repair = notification.Repair;

        var payload = new
        {
            WorkshopId = repair.WorkshopId,
            EstimatedCost = repair.EstimatedCost.Amount,
            Currency = repair.EstimatedCost.Currency,
            FinalCost = repair.FinalCost?.Amount,
            FinalCostCurrency = repair.FinalCost?.Currency,
            Status = repair.Status.ToString()
        };

        return CreateTimelineEntryAsync(
            repair.VehicleId,
            nameof(RepairCompletedDomainEvent),
            payload,
            cancellationToken);
    }
}

