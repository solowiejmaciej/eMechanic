namespace eMechanic.Events.Events.RepairRequest;

using System;

public class RepairRequestEstimatedEvent : EventBase
{
    public Guid RepairRequestId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public decimal EstimatedCostAmount { get; private set; }
    public string EstimatedCostCurrency { get; private set; }
    public string Diagnosis { get; private set; }

    public RepairRequestEstimatedEvent(Guid repairRequestId, Guid userId, Guid vehicleId, decimal estimatedCostAmount, string estimatedCostCurrency, string diagnosis)
    {
        RepairRequestId = repairRequestId;
        UserId = userId;
        VehicleId = vehicleId;
        EstimatedCostAmount = estimatedCostAmount;
        EstimatedCostCurrency = estimatedCostCurrency;
        Diagnosis = diagnosis;
    }
}
