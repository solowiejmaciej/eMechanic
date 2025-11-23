
namespace eMechanic.Application.RepairRequest.Features.Get.ForUser;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;
using Vehicle.Services;

public sealed class GetRepairRequestsForUserVehicleHandler : IResultQueryHandler<GetRepairRequestsForUserVehicleQuery, PaginationResult<RepairRequestResponse>>
{
    private readonly IVehicleOwnershipService _vehicleOwnershipService;
    private readonly IRepairRequestRepository _repairRequestRepository;

    public GetRepairRequestsForUserVehicleHandler(IVehicleOwnershipService vehicleOwnershipService, IRepairRequestRepository repairRequestRepository)
    {
        _vehicleOwnershipService = vehicleOwnershipService;
        _repairRequestRepository = repairRequestRepository;
    }

    public async Task<Result<PaginationResult<RepairRequestResponse>, Error>> Handle(GetRepairRequestsForUserVehicleQuery request, CancellationToken cancellationToken)
    {
        var ownershipResult = await _vehicleOwnershipService.VerifyOwnershipAsync(request.VehicleId, cancellationToken);

        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        var repairRequests = await _repairRequestRepository.GetForUserVehicleAsync(request.VehicleId, request.Pagination, cancellationToken);

        var result = repairRequests.MapToDto(rr => new RepairRequestResponse(
            rr.Id,
            rr.VehicleId,
            rr.WorkshopId,
            rr.Description.Value,
            rr.Diagnosis?.Value,
            rr.EstimatedCost?.Amount,
            rr.EstimatedCost?.Currency,
            rr.Status,
            rr.RejectionReason,
            rr.CreatedAt,
            rr.SummaryReport
        ));

        return result;
    }
}
