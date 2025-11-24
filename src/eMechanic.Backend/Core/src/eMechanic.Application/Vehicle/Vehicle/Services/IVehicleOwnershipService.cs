namespace eMechanic.Application.Vehicle.Vehicle.Services;

using Domain.Vehicle.Vehicle;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;

public interface IVehicleOwnershipService
{
    Task<Result<Vehicle, Error>> GetAndVerifyOwnershipAsync(
        Guid vehicleId,
        CancellationToken cancellationToken);

    Task<Result<Success, Error>> VerifyOwnershipAsync(
        Guid vehicleId,
        CancellationToken cancellationToken);
}
