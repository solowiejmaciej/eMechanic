
namespace eMechanic.Application.RepairRequest.Features.Accept;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;
using Vehicle.Services;

public class AcceptRepairEstimationHandler : IResultCommandHandler<AcceptRepairEstimationCommand, Success>
{
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IVehicleOwnershipService _vehicleOwnershipService;

    public AcceptRepairEstimationHandler(
        IRepairRequestRepository repairRequestRepository,
        IVehicleOwnershipService vehicleOwnershipService)
    {
        _repairRequestRepository = repairRequestRepository;
        _vehicleOwnershipService = vehicleOwnershipService;
    }

    public async Task<Result<Success,Error>> Handle(AcceptRepairEstimationCommand request, CancellationToken cancellationToken)
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

        var acceptResult = repairRequest.AcceptEstimation();

        if (acceptResult.HasError())
        {
            return acceptResult.Error!;
        }

        await _repairRequestRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
