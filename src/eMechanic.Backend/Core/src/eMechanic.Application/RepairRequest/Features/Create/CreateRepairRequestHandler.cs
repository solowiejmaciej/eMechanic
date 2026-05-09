
namespace eMechanic.Application.RepairRequest.Features.Create;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;
using Domain.RepairRequest;
using Services;
using Vehicle.Vehicle.Services;

public sealed class CreateRepairRequestHandler : IResultCommandHandler<CreateRepairRequestCommand, Guid>
{
    private readonly IVehicleOwnershipService _vehicleOwnershipService;
    private readonly IRepairRequestRepository _repairRequestRepository;

    public CreateRepairRequestHandler(
        IVehicleOwnershipService vehicleOwnershipService,
        IRepairRequestRepository repairRequestRepository)
    {
        _vehicleOwnershipService = vehicleOwnershipService;
        _repairRequestRepository = repairRequestRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateRepairRequestCommand request, CancellationToken cancellationToken)
    {
        var ownershipResult =
            await _vehicleOwnershipService.GetAndVerifyOwnershipAsync(request.VehicleId, cancellationToken);

        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        var vehicle = ownershipResult.Value!;

        var repairRequestResult = RepairRequest.Create(
            vehicle.UserId,
            vehicle.Id,
            request.WorkshopId,
            request.Description);

        if (repairRequestResult.HasError())
        {
            return repairRequestResult.Error!;
        }

        var repairRequest = repairRequestResult.Value!;

        await _repairRequestRepository.AddAsync(repairRequest, cancellationToken);
        await _repairRequestRepository.SaveChangesAsync(cancellationToken);
        return repairRequest.Id;
    }
}
