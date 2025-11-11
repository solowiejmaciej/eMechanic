namespace eMechanic.Application.Vehicle.Services;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;
using Repostories;

public sealed class VehicleOwnershipService : IVehicleOwnershipService
{
    private readonly IUserContext _userContext;
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleOwnershipService(
        IUserContext userContext,
        IVehicleRepository vehicleRepository)
    {
        _userContext = userContext;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result<Vehicle, Error>> GetAndVerifyOwnershipAsync(
        Guid vehicleId,
        CancellationToken cancellationToken)
    {
        Guid currentUserId;
        try
        {
            currentUserId = _userContext.GetUserId();
        }
        catch (UnauthorizedAccessException ex)
        {
            return new Error(EErrorCode.UnauthorizedError, ex.Message);
        }

        var vehicle = await _vehicleRepository.GetForUserById(
            vehicleId,
            currentUserId,
            cancellationToken);

        if (vehicle is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{vehicleId}' not found.");
        }

        return vehicle;
    }
}
