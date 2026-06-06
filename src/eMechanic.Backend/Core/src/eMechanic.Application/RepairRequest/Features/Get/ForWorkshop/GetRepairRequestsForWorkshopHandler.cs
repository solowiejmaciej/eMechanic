
namespace eMechanic.Application.RepairRequest.Features.Get.ForWorkshop;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class GetRepairRequestsForWorkshopHandler : IResultQueryHandler<GetRepairRequestsForWorkshopQuery, PaginationResult<RepairRequestResponse>>
{
    private readonly IWorkshopContext _workshopContext;
    private readonly IRepairRequestRepository _repairRequestRepository;

    public GetRepairRequestsForWorkshopHandler(IWorkshopContext workshopContext, IRepairRequestRepository repairRequestRepository)
    {
        _workshopContext = workshopContext;
        _repairRequestRepository = repairRequestRepository;
    }

    public async Task<Result<PaginationResult<RepairRequestResponse>, Error>> Handle(GetRepairRequestsForWorkshopQuery request, CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();

        var repairRequests = await _repairRequestRepository.GetForWorkshopAsync(workshopId, request.Pagination, cancellationToken);

        var result = repairRequests.MapToDto(rr => new RepairRequestResponse(
            rr.Id,
            rr.VehicleId,
            rr.WorkshopId,
            rr.Description.Value,
            rr.Diagnosis?.Value,
            rr.EstimatedCost?.Amount,
            rr.EstimatedCost?.Currency,
            rr.Status,
            rr.RejectionReason?.Value,
            rr.CreatedAt,
            rr.SummaryReport?.Value
        ));

        return result;
    }
}
