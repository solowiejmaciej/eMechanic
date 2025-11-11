namespace eMechanic.Application.Vehicle.Repostories;

using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.VehicleTimeline;

public interface IVehicleTimelineRepository : IRepository<VehicleTimeline>
{
    Task<PaginationResult<VehicleTimeline>> GetByVehicleIdPaginatedAsync(Guid vehicleId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
}
