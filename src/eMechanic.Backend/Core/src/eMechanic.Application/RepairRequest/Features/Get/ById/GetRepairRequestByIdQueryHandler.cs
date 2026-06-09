namespace eMechanic.Application.RepairRequest.Features.Get.ById;

using Common.CQRS;
using Common.Result;
using Repositories;
using eMechanic.Domain.RepairRequest.Enums;

public sealed class GetRepairRequestByIdQueryHandler : IResultQueryHandler<GetRepairRequestByIdQuery, RepairRequestResponse>
{
    private readonly IRepairRequestRepository _repairRequestRepository;

    public GetRepairRequestByIdQueryHandler(IRepairRequestRepository repairRequestRepository)
    {
        _repairRequestRepository = repairRequestRepository;
    }

    //TODO Verify ownership
    public async Task<Result<RepairRequestResponse, Error>> Handle(
        GetRepairRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var repairRequest = await _repairRequestRepository.GetByIdAsync(request.Id, cancellationToken);

        if (repairRequest is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair request with ID '{request.Id}' was not found.");
        }

        return new RepairRequestResponse(
            repairRequest.Id,
            repairRequest.VehicleId,
            repairRequest.WorkshopId,
            repairRequest.Description.Value,
            repairRequest.Diagnosis?.Value,
            repairRequest.EstimatedCost?.Amount,
            repairRequest.EstimatedCost?.Currency,
            repairRequest.Status,
            repairRequest.RejectionReason?.Value,
            repairRequest.CreatedAt,
            repairRequest.SummaryReport?.Value
        );
    }
}
