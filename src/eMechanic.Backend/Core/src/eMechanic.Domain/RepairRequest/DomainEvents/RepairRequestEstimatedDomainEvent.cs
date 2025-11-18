namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;
using Events.Events;
using Events.Events.RepairRequest;

public record RepairRequestEstimatedDomainEvent(RepairRequest RepairRequest) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent()
    {
        return new RepairRequestEstimatedEvent(
            RepairRequest.Id,
            RepairRequest.UserId,
            RepairRequest.VehicleId,
            RepairRequest.EstimatedCost!.Amount,
            RepairRequest.EstimatedCost.Currency,
            RepairRequest.Diagnosis!.Value);
    }
}
