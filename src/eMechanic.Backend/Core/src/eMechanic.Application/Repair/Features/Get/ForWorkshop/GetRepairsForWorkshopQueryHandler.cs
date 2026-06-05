namespace eMechanic.Application.Repair.Features.Get.ForWorkshop;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using eMechanic.Application.Repair.Features.Get;
using Repositories;

public sealed class GetRepairsForWorkshopQueryHandler : IResultQueryHandler<GetRepairsForWorkshopQuery, PaginationResult<RepairListItemResponse>>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IWorkshopContext _workshopContext;

    public GetRepairsForWorkshopQueryHandler(IRepairRepository repairRepository, IWorkshopContext workshopContext)
    {
        _repairRepository = repairRepository;
        _workshopContext = workshopContext;
    }

    public async Task<Result<PaginationResult<RepairListItemResponse>, Error>> Handle(
        GetRepairsForWorkshopQuery request,
        CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();
        var repairs = await _repairRepository.GetForWorkshopPaginatedAsync(workshopId, request.Pagination, cancellationToken);

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

