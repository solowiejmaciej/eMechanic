namespace eMechanic.Application.Vehicle.Vehicle.Repostories;

using Domain.Vehicle.Timeline;
using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Common.Result;

public interface IVehicleTimelineRepository : IRepository<VehicleTimeline>
{
    Task<PaginationResult<VehicleTimeline>> GetByVehicleIdPaginatedAsync(Guid vehicleId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
}
