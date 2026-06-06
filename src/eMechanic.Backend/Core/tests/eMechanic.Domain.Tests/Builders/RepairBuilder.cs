namespace eMechanic.Domain.Tests.Builders;

using eMechanic.Common.Result;
using eMechanic.Domain.Repair.Enums;
using eMechanic.Domain.Shared.ValueObjects;

public sealed class RepairBuilder
{
    private Guid _repairRequestId = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _workshopId = Guid.NewGuid();
    private Money _estimatedCost = Money.Create(100m, "PLN").Value!;
    private ERepairStatus _status = ERepairStatus.Scheduled;
    private Money _finalCost = Money.Create(120m, "PLN").Value!;

    public RepairBuilder WithRepairRequestId(Guid? repairRequestId)
    {
        _repairRequestId = repairRequestId ?? Guid.Empty;
        return this;
    }

    public RepairBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public RepairBuilder WithWorkshopId(Guid workshopId)
    {
        _workshopId = workshopId;
        return this;
    }

    public RepairBuilder WithEstimatedCost(decimal amount, string currency = "PLN")
    {
        _estimatedCost = Money.Create(amount, currency).Value!;
        return this;
    }

    public RepairBuilder WithFinalCost(decimal amount, string currency = "PLN")
    {
        _finalCost = Money.Create(amount, currency).Value!;
        return this;
    }

    public RepairBuilder WithStatus(ERepairStatus status)
    {
        _status = status;
        return this;
    }

    public Domain.Repair.Repair Build()
    {
        var repairResult = BuildResult();
        if (repairResult.HasError())
        {
            throw new InvalidOperationException("Failed to create a valid repair with provided builder values.");
        }

        return repairResult.Value!;
    }

    public Result<Domain.Repair.Repair, Error> BuildResult()
    {
        Guid? repairRequestId = _repairRequestId == Guid.Empty ? null : _repairRequestId;
        var createResult = Domain.Repair.Repair.Create(_vehicleId, _workshopId, _estimatedCost, repairRequestId);

        if (createResult.HasError())
        {
            return createResult;
        }

        var repair = createResult.Value!;

        if (_status == ERepairStatus.InProgress)
        {
            repair.StartRepair();
        }
        else if (_status == ERepairStatus.Completed)
        {
            repair.StartRepair();
            repair.CompleteRepair(_finalCost);
        }
        else if (_status == ERepairStatus.Paid)
        {
            repair.StartRepair();
            repair.CompleteRepair(_finalCost);
            repair.Pay();
        }

        return repair;
    }
}


