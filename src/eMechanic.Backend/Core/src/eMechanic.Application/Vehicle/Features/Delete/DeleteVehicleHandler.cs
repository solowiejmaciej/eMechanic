namespace eMechanic.Application.Vehicle.Delete;

using Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Vehicle;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public sealed class DeleteVehicleHandler : IResultCommandHandler<DeleteVehicleCommand, Success>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;

    public DeleteVehicleHandler(
        IVehicleRepository vehicleRepository,
        IUserContext userContext)
    {
        _vehicleRepository = vehicleRepository;
        _userContext = userContext;
    }

    public async Task<Result<Success, Error>> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.GetUserId();
        var vehicle = await _vehicleRepository.GetForUserById(request.Id, currentUserId, cancellationToken);

        if (vehicle is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{request.Id}' not found.");
        }

        _vehicleRepository.DeleteAsync(vehicle, cancellationToken);
        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
