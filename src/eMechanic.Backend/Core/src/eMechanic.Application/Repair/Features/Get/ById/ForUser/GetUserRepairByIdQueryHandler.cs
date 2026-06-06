namespace eMechanic.Application.Repair.Features.Get.ById.ForUser;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class GetUserRepairByIdQueryHandler : IResultQueryHandler<GetUserRepairByIdQuery, RepairResponse>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IUserContext _userContext;


    public GetUserRepairByIdQueryHandler(IRepairRepository repairRepository, IUserContext userContext)
    {
        _repairRepository = repairRepository;
        _userContext = userContext;
    }

    public async Task<Result<RepairResponse, Error>> Handle(GetUserRepairByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();
        var repair = await _repairRepository.GetForUserByIdAsNoTrackingAsync(userId, request.RepairId, cancellationToken);

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

