namespace eMechanic.Domain.RepairRequest;

using System;
using Common.Attributes;
using Common.DDD;
using Common.Result;
using Enums;
using ValueObjects;
using Shared.ValueObjects;
using DomainEvents;
using Shared.References.User;
using Shared.References.Vehicle;
using Shared.References.Workshop;

public class RepairRequest : AggregateRoot, IVehicleReference, IWorkshopReference, IUserReferenced
{
    public Guid VehicleId { get; private set; }
    public Guid WorkshopId { get; private set; }
    public Guid UserId { get; private set; }

    [Searchable]
    public RepairDescription Description { get; private set; }

    [Searchable]
    public RepairDiagnosis? Diagnosis { get; private set; }
    public Money? EstimatedCost { get; private set; }

    public ERepairRequestStatus Status { get; private set; }

    [Searchable]
    public string? RejectionReason { get; private set; }

    [Searchable]
    public string? SummaryReport { get; private set; }

    private RepairRequest() { }

    private RepairRequest(
        Guid userId,
        Guid vehicleId,
        Guid workshopId,
        RepairDescription description)
    {
        UserId = userId;
        VehicleId = vehicleId;
        WorkshopId = workshopId;
        Description = description;
        Status = ERepairRequestStatus.Pending;

        RaiseDomainEvent(new RepairRequestCreatedDomainEvent(this));
    }

    public static Result<RepairRequest, Error> Create(
        Guid userId,
        Guid vehicleId,
        Guid workshopId,
        string descriptionString)
    {
        if (userId == Guid.Empty)
            return new Error(EErrorCode.ValidationError, "UserId cannot be empty.");

        if (vehicleId == Guid.Empty)
            return new Error(EErrorCode.ValidationError, "VehicleId cannot be empty.");

        if (workshopId == Guid.Empty)
            return new Error(EErrorCode.ValidationError, "WorkshopId cannot be empty.");

        var descriptionResult = RepairDescription.Create(descriptionString);
        if (descriptionResult.HasError())
            return descriptionResult.Error!;

        return new RepairRequest(userId, vehicleId, workshopId, descriptionResult.Value!);
    }

    public Result<Success, Error> ProvideEstimation(string diagnosisString, decimal costAmount, string currency = "PLN")
    {
        if (Status != ERepairRequestStatus.Pending)
        {
            return new Error(EErrorCode.ValidationError, $"Estimation can only be provided for requests in '{ERepairRequestStatus.Pending}' status. Current status: '{Status}'.");
        }

        var diagnosisResult = RepairDiagnosis.Create(diagnosisString);
        if (diagnosisResult.HasError()) return diagnosisResult.Error!;

        var moneyResult = Money.Create(costAmount, currency);
        if (moneyResult.HasError()) return moneyResult.Error!;

        Diagnosis = diagnosisResult.Value;
        EstimatedCost = moneyResult.Value;
        Status = ERepairRequestStatus.Estimated;

        RaiseDomainEvent(new RepairRequestEstimatedDomainEvent(this));

        return Result.Success;
    }

    public Result<Success, Error> AcceptEstimation()
    {
        if (Status != ERepairRequestStatus.Estimated)
        {
            return new Error(EErrorCode.ValidationError, "Only requests with provided estimation can be accepted.");
        }

        Status = ERepairRequestStatus.Accepted;
        RaiseDomainEvent(new RepairRequestAcceptedDomainEvent(this));

        return Result.Success;
    }

    public Result<Success, Error> RejectEstimation(string reason)
    {
        if (Status != ERepairRequestStatus.Estimated)
        {
            return new Error(EErrorCode.ValidationError, "Only requests with provided estimation can be rejected.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
             return new Error(EErrorCode.ValidationError, "Rejection reason is required.");
        }

        Status = ERepairRequestStatus.Rejected;
        RejectionReason = reason;
        RaiseDomainEvent(new RepairRequestRejectedDomainEvent(this));

        return Result.Success;
    }

    public void SetSummaryReport(string summaryReport) => SummaryReport = summaryReport;
}
