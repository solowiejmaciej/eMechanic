namespace eMechanic.Application.Vehicle.Services;

using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;

public interface IVehicleOwnershipService
{
    Task<Result<Vehicle, Error>> GetAndVerifyOwnershipAsync(
        Guid vehicleId,
        CancellationToken cancellationToken);
}
