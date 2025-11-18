namespace eMechanic.Events.Events.RepairRequest;

using System;

public class RepairRequestCreatedEvent : EventBase
{
    public Guid RepairRequestId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid WorkshopId { get; private set; }
    public string Description { get; private set; }

    public RepairRequestCreatedEvent(Guid repairRequestId, Guid userId, Guid vehicleId, Guid workshopId, string description)
    {
        RepairRequestId = repairRequestId;
        UserId = userId;
        VehicleId = vehicleId;
        WorkshopId = workshopId;
        Description = description;
    }
}
