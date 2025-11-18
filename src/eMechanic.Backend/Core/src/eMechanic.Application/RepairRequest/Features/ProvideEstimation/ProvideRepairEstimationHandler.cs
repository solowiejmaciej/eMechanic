
namespace eMechanic.Application.RepairRequest.Features.ProvideEstimation;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;

public class ProvideRepairEstimationHandler : IResultCommandHandler<ProvideRepairEstimationCommand, Success>
{
    private readonly IWorkshopContext _workshopContext;
    private readonly IRepairRequestRepository _repairRequestRepository;

    public ProvideRepairEstimationHandler(IWorkshopContext workshopContext,
        IRepairRequestRepository repairRequestRepository)
    {
        _workshopContext = workshopContext;
        _repairRequestRepository = repairRequestRepository;
    }

    public async Task<Result<Success, Error>> Handle(ProvideRepairEstimationCommand request, CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();

        var repairRequest = await _repairRequestRepository.GetByIdAsync(request.RepairRequestId, cancellationToken);

        if (repairRequest is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair request with ID {request.RepairRequestId} not found.");
        }

        if (repairRequest.WorkshopId != workshopId)
        {
            return new Error(EErrorCode.UnauthorizedError, "Workshop is not assigned to this repair request.");
        }

        var estimationResult = repairRequest.ProvideEstimation(request.Diagnosis, request.Cost, request.Currency);

        if (estimationResult.HasError())
        {
            return estimationResult.Error!;
        }

        await _repairRequestRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
