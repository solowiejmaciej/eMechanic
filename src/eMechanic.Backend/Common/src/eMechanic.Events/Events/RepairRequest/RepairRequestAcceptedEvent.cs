namespace eMechanic.Events.Events.RepairRequest;

using System;

public class RepairRequestAcceptedEvent : EventBase
{
    public Guid RepairRequestId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }

    public RepairRequestAcceptedEvent(Guid repairRequestId, Guid userId, Guid vehicleId)
    {
        RepairRequestId = repairRequestId;
        UserId = userId;
        VehicleId = vehicleId;
    }
}
