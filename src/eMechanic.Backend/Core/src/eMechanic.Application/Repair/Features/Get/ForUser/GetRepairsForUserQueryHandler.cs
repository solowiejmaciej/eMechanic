namespace eMechanic.Application.Repair.Features.Get.ForUser;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using eMechanic.Application.Repair.Features.Get;
using Repositories;

public sealed class GetRepairsForUserQueryHandler : IResultQueryHandler<GetRepairsForUserQuery, PaginationResult<RepairListItemResponse>>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IUserContext _userContext;

    public GetRepairsForUserQueryHandler(IRepairRepository repairRepository, IUserContext userContext)
    {
        _repairRepository = repairRepository;
        _userContext = userContext;
    }

    public async Task<Result<PaginationResult<RepairListItemResponse>, Error>> Handle(
        GetRepairsForUserQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();
        var repairs = await _repairRepository.GetForUserPaginatedAsync(userId, request.Pagination, cancellationToken);

        return repairs.MapToDto(r => new RepairListItemResponse(
            r.Id,
            r.RepairRequestId,
            r.VehicleId,
            r.WorkshopId,
            r.Status.ToString(),
            new ById.RepairMoneyResponse(r.EstimatedCost.Amount, r.EstimatedCost.Currency),
            r.FinalCost is null ? null : new ById.RepairMoneyResponse(r.FinalCost.Amount, r.FinalCost.Currency),
            r.CreatedAt));
    }
}

