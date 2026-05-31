namespace eMechanic.Application.Repair.Features.Get.ById;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class GetRepairByIdQueryHandler : IResultQueryHandler<GetRepairByIdQuery, RepairResponse>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IUserContext _userContext;
    private readonly IWorkshopContext _workshopContext;

    public GetRepairByIdQueryHandler(
        IRepairRepository repairRepository,
        IUserContext userContext,
        IWorkshopContext workshopContext)
    {
        _repairRepository = repairRepository;
        _userContext = userContext;
        _workshopContext = workshopContext;
    }

    public async Task<Result<RepairResponse, Error>> Handle(GetRepairByIdQuery request, CancellationToken cancellationToken)
    {
        if (TryGetUserId(out var userId))
        {
            var userRepair = await _repairRepository.GetForUserByIdAsNoTrackingAsync(userId, request.RepairId, cancellationToken);
            if (userRepair is not null)
            {
                return MapToResponse(userRepair);
            }
        }

        if (TryGetWorkshopId(out var workshopId))
        {
            var workshopRepair = await _repairRepository.GetForWorkshopByIdAsNoTrackingAsync(workshopId, request.RepairId, cancellationToken);
            if (workshopRepair is not null)
            {
                return MapToResponse(workshopRepair);
            }
        }

        return new Error(EErrorCode.NotFoundError, $"Repair with ID {request.RepairId} not found.");
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;

        try
        {
            userId = _userContext.GetUserId();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private bool TryGetWorkshopId(out Guid workshopId)
    {
        workshopId = Guid.Empty;

        try
        {
            workshopId = _workshopContext.GetWorkshopId();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static RepairResponse MapToResponse(Domain.Repair.Repair repair)
    {
        return new RepairResponse(
            repair.Id,
            repair.RepairRequestId,
            repair.VehicleId,
            repair.WorkshopId,
            repair.Status.ToString(),
            new RepairMoneyResponse(repair.EstimatedCost.Amount, repair.EstimatedCost.Currency),
            repair.FinalCost is null
                ? null
                : new RepairMoneyResponse(repair.FinalCost.Amount, repair.FinalCost.Currency));
    }
}

