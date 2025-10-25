namespace eMechanic.Application.Abstractions.Vehicle;

using Domain.Vehicle;
using Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetForUserById(Guid entityId, Guid userId, CancellationToken cancellationToken);
}
