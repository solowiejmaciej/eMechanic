
namespace eMechanic.Application.RepairRequest.Features.Reject;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;
using Vehicle.Services;

public class RejectRepairEstimationHandler : IResultCommandHandler<RejectRepairEstimationCommand, Success>
{
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IVehicleOwnershipService _vehicleOwnershipService;

    public RejectRepairEstimationHandler(IUserContext userContext,
        IRepairRequestRepository repairRequestRepository,
        IVehicleOwnershipService vehicleOwnershipService)
    {
        _repairRequestRepository = repairRequestRepository;
        _vehicleOwnershipService = vehicleOwnershipService;
    }

    public async Task<Result<Success, Error>> Handle(RejectRepairEstimationCommand request, CancellationToken cancellationToken)
    {
        var repairRequest = await _repairRequestRepository.GetByIdAsync(request.RepairRequestId, cancellationToken);

        if (repairRequest is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair request with ID {request.RepairRequestId} not found.");
        }

        var ownershipResult =
            await _vehicleOwnershipService.GetAndVerifyOwnershipAsync(repairRequest.VehicleId, cancellationToken);

        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        var rejectResult = repairRequest.RejectEstimation(request.Reason);

        if (rejectResult.HasError())
        {
            return rejectResult.Error!;
        }

        await _repairRequestRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
