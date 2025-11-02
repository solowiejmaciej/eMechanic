namespace eMechanic.Infrastructure.Repositories;

using Application.Abstractions.VehicleTimeline;
using Base;
using Common.Result;
using DAL;
using Domain.VehicleTimeline;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Services;

internal sealed class VehicleTimelineRepository : Repository<VehicleTimeline>, IVehicleTimelineRepository
{
    public VehicleTimelineRepository(AppDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
    }

    public async Task<PaginationResult<VehicleTimeline>> GetByVehicleIdPaginatedAsync(Guid vehicleId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQuery()
            .FilterByVehicleId(vehicleId);

        return await GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }
}
