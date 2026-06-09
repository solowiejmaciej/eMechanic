namespace eMechanic.Domain.Repair;

using Common.Attributes;
using Common.DDD;
using Common.Result;
using DomainEvents;
using Enums;
using Shared.ValueObjects;

public class Repair : AggregateRoot
{
    public Guid? RepairRequestId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid WorkshopId { get; private set; }
    public Money EstimatedCost { get; private set; } = null!;
    public Money? FinalCost { get; private set; }

    [Searchable]
    public ERepairStatus Status { get; private set; }

    private Repair()
    {
    }

    private Repair(
        Guid vehicleId,
        Guid workshopId,
        Money estimatedCost,
        Guid? repairRequestId)
    {
        VehicleId = vehicleId;
        WorkshopId = workshopId;
        EstimatedCost = estimatedCost;
        RepairRequestId = repairRequestId;
        Status = ERepairStatus.Scheduled;

        RaiseDomainEvent(new RepairCreatedDomainEvent(this));
    }

    public static Result<Repair, Error> Create(
        Guid vehicleId,
        Guid workshopId,
        Money estimatedCost,
        Guid? repairRequestId = null)
    {
        if (vehicleId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "VehicleId cannot be empty.");
        }

        if (workshopId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "WorkshopId cannot be empty.");
        }

        if (repairRequestId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "RepairRequestId cannot be empty when provided.");
        }

        return new Repair(vehicleId, workshopId, estimatedCost, repairRequestId);
    }

    public Result<Success, Error> StartRepair()
    {
        if (Status != ERepairStatus.Scheduled)
        {
            return new Error(
                EErrorCode.ValidationError,
                $"Repair can be started only from '{ERepairStatus.Scheduled}' status. Current status: '{Status}'.");
        }

        Status = ERepairStatus.InProgress;
        RaiseDomainEvent(new RepairStartedDomainEvent(this));

        return Result.Success;
    }

    public Result<Success, Error> CompleteRepair(Money finalCost)
    {
        if (Status != ERepairStatus.InProgress)
        {
            return new Error(
                EErrorCode.ValidationError,
                $"Repair can be completed only from '{ERepairStatus.InProgress}' status. Current status: '{Status}'.");
        }

        FinalCost = finalCost;
        Status = ERepairStatus.Completed;
        RaiseDomainEvent(new RepairCompletedDomainEvent(this));

        return Result.Success;
    }

    public Result<Success, Error> Pay()
    {
        if (Status != ERepairStatus.Completed)
        {
            return new Error(
                EErrorCode.ValidationError,
                $"Repair can be paid only from '{ERepairStatus.Completed}' status. Current status: '{Status}'.");
        }

        Status = ERepairStatus.Paid;
        RaiseDomainEvent(new RepairPaidDomainEvent(this));

        return Result.Success;
    }
}

