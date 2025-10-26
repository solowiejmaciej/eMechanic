namespace eMechanic.Application.Abstractions.Vehicle;

using Common.Result;
using Domain.Vehicle;
using Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetForUserById(Guid entityId, Guid userId, CancellationToken cancellationToken);
    Task<PaginationResult<Vehicle>> GetForUserPaginatedAsync(PaginationParameters requestPaginationParameters,
        Guid userId, CancellationToken cancellationToken);
}
