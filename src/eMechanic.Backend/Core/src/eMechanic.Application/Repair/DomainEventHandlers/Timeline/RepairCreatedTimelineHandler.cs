namespace eMechanic.Application.Repair.DomainEventHandlers.Timeline;

using Abstractions.DomainEvents;
using eMechanic.Application.Timeline;
using eMechanic.Application.Vehicle.Vehicle.Repositories;
using eMechanic.Domain.Repair.DomainEvents;

public class RepairCreatedTimelineHandler : BaseTimelineEventHandler,
    IDomainEventHandler<RepairCreatedDomainEvent>
{
    public RepairCreatedTimelineHandler(IVehicleTimelineRepository vehicleTimelineRepository) : base(
        vehicleTimelineRepository)
    {
    }

    public Task Handle(RepairCreatedDomainEvent notification, CancellationToken cancellationToken)
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
            nameof(RepairCreatedDomainEvent),
            payload,
            cancellationToken);
    }
}

