namespace eMechanic.Application.Repair.Features.Get.ById;

public sealed record RepairMoneyResponse(decimal Amount, string Currency);

public sealed record RepairResponse(
    Guid Id,
    Guid? RepairRequestId,
    Guid VehicleId,
    Guid WorkshopId,
    string Status,
    RepairMoneyResponse EstimatedCost,
    RepairMoneyResponse? FinalCost);

