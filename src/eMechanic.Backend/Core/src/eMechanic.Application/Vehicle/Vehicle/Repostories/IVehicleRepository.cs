namespace eMechanic.Application.Vehicle.Vehicle.Repostories;

using Domain.Vehicle.Vehicle;
using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetForUserById(Guid entityId, Guid userId, CancellationToken cancellationToken);
    Task<PaginationResult<Vehicle>> GetForUserPaginatedAsync(PaginationParameters paginationParameters,
        Guid userId, CancellationToken cancellationToken);

    Task<bool> ExistsForUserAsync(Guid vehicleId, Guid currentUserId, CancellationToken cancellationToken);
}
