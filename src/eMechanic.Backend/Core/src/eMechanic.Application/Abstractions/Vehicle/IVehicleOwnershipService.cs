namespace eMechanic.Application.Abstractions.Vehicle;

using Common.Result;
using Domain.Vehicle;

public interface IVehicleOwnershipService
{
    Task<Result<Vehicle, Error>> GetAndVerifyOwnershipAsync(
        Guid vehicleId,
        CancellationToken cancellationToken);
}
