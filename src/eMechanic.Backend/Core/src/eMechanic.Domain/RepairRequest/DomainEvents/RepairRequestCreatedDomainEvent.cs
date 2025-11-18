namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;
using Events.Events;
using Events.Events.RepairRequest;

public record RepairRequestCreatedDomainEvent(RepairRequest RepairRequest) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent() => new RepairRequestCreatedEvent(
        RepairRequest.Id,
        RepairRequest.UserId,
        RepairRequest.VehicleId,
        RepairRequest.WorkshopId,
        RepairRequest.Description.Value);
}
