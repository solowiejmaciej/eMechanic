namespace eMechanic.Application.Repair.Features.Get.ById;

using Domain.Repair.Enums;

public sealed record RepairMoneyResponse(decimal Amount, string Currency);

public sealed record RepairResponse(
    Guid Id,
    Guid? RepairRequestId,
    Guid VehicleId,
    Guid WorkshopId,
    ERepairStatus Status,
    RepairMoneyResponse EstimatedCost,
    RepairMoneyResponse? FinalCost);

