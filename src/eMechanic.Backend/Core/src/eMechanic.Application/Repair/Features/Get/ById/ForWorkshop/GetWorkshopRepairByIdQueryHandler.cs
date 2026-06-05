namespace eMechanic.Application.Repair.Features.Get.ById.ForWorkshop;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class GetWorkshopRepairByIdQueryHandler : IResultQueryHandler<GetWorkshopRepairByIdQuery, RepairResponse>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IWorkshopContext _workshopContext;

    public GetWorkshopRepairByIdQueryHandler(IRepairRepository repairRepository, IWorkshopContext workshopContext)
    {
        _repairRepository = repairRepository;
        _workshopContext = workshopContext;
    }

    public async Task<Result<RepairResponse, Error>> Handle(GetWorkshopRepairByIdQuery request, CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();
        var repair = await _repairRepository.GetForWorkshopByIdAsNoTrackingAsync(workshopId, request.RepairId, cancellationToken);

        if (repair is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair with ID {request.RepairId} not found.");
        }

        return new RepairResponse(
            repair.Id,
            repair.RepairRequestId,
            repair.VehicleId,
            repair.WorkshopId,
            repair.Status,
            new RepairMoneyResponse(repair.EstimatedCost.Amount, repair.EstimatedCost.Currency),
            repair.FinalCost is null
                ? null
                : new RepairMoneyResponse(repair.FinalCost.Amount, repair.FinalCost.Currency));
    }
}

