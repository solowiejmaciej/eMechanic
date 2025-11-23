
using eMechanic.Application.RepairRequest.Repositories;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

namespace eMechanic.Application.RepairRequest.Features.Get;

public sealed class GetRepairRequestByIdHandler : IResultQueryHandler<GetRepairRequestByIdQuery, RepairRequestResponse>
{
    private readonly IRepairRequestRepository _repairRequestRepository;

    public GetRepairRequestByIdHandler(IRepairRequestRepository repairRequestRepository)
    {
        _repairRequestRepository = repairRequestRepository;
    }

    public async Task<Result<RepairRequestResponse, Error>> Handle(GetRepairRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var repairRequest = await _repairRequestRepository.GetByIdAsync(request.Id, cancellationToken);

        if (repairRequest is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair request with ID {request.Id} not found.");
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
            repairRequest.RejectionReason,
            repairRequest.CreatedAt,
            repairRequest.SummaryReport
        );
    }
}
