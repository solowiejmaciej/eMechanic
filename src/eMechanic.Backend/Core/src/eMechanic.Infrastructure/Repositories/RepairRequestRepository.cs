
namespace eMechanic.Infrastructure.Repositories;

using Application.RepairRequest.Repositories;
using Base;
using Common.Result;
using DAL;
using Domain.RepairRequest;
using Extensions;
using Services;

internal sealed class RepairRequestRepository : Repository<RepairRequest>, IRepairRequestRepository
{
    public RepairRequestRepository(AppDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
    }

    public Task<PaginationResult<RepairRequest>> GetForUserVehicleAsync(Guid vehicleId, PaginationParameters paginationParameters, CancellationToken cancellationToken)
    {
        var query = GetQuery()
            .FilterByVehicleId(vehicleId);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }

    public Task<PaginationResult<RepairRequest>> GetForWorkshopAsync(Guid workshopId, PaginationParameters paginationParameters, CancellationToken cancellationToken)
    {
        var query = GetQuery()
            .FilterByWorkshopId(workshopId);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }
}
