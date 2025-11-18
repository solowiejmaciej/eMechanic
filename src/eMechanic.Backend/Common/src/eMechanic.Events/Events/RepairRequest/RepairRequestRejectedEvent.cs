namespace eMechanic.Events.Events.RepairRequest;

using System;

public class RepairRequestRejectedEvent : EventBase
{
    public Guid RepairRequestId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string RejectionReason { get; private set; }

    public RepairRequestRejectedEvent(Guid repairRequestId, Guid userId, Guid vehicleId, string rejectionReason)
    {
        RepairRequestId = repairRequestId;
        UserId = userId;
        VehicleId = vehicleId;
        RejectionReason = rejectionReason;
    }
}
