namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;
using Events.Events;
using Events.Events.RepairRequest;

public record RepairRequestAcceptedDomainEvent(RepairRequest RepairRequest) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent() => new RepairRequestAcceptedEvent(
        RepairRequest.Id,
        RepairRequest.UserId,
        RepairRequest.VehicleId);
}
