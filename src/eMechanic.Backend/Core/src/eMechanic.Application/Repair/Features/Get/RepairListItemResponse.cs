namespace eMechanic.Application.Repair.Features.Get;

using ById;

public sealed record RepairListItemResponse(
    Guid Id,
    Guid? RepairRequestId,
    Guid VehicleId,
    Guid WorkshopId,
    string Status,
    RepairMoneyResponse EstimatedCost,
    RepairMoneyResponse? FinalCost,
    DateTime CreatedAt);

