namespace eMechanic.Domain.RepairRequest.DomainEvents;

using Common.DDD;
using Events.Events;
using Events.Events.RepairRequest;
using System;

public record RepairRequestRejectedDomainEvent(RepairRequest RepairRequest) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent()
    {
        return new RepairRequestRejectedEvent(
            RepairRequest.Id,
            RepairRequest.UserId,
            RepairRequest.VehicleId,
            RepairRequest.RejectionReason!);
    }
}
