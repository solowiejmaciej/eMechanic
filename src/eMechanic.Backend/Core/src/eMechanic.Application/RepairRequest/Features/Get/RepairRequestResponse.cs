
namespace eMechanic.Application.RepairRequest.Features.Get;

using Domain.RepairRequest.Enums;

public record RepairRequestResponse(
    Guid Id,
    Guid VehicleId,
    Guid WorkshopId,
    string Description,
    string? Diagnosis,
    decimal? EstimatedCostAmount,
    string? EstimatedCostCurrency,
    ERepairRequestStatus Status,
    string? RejectionReason,
    DateTime CreatedAt);
