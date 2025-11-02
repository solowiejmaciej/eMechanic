namespace eMechanic.Application.Abstractions.VehicleTimeline;

using Common.Result;
using Domain.VehicleTimeline;
using Repositories;

public interface IVehicleTimelineRepository : IRepository<VehicleTimeline>
{
    Task<PaginationResult<VehicleTimeline>> GetByVehicleIdPaginatedAsync(Guid vehicleId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
}
