namespace eMechanic.Infrastructure.Repositories;

using Application.VehicleDocument.Repositories;
using Base;
using Common.Result;
using DAL;
using Domain.VehicleDocument;
using Extensions;
using Services;

internal sealed class VehicleDocumentRepository : Repository<VehicleDocument>, IVehicleDocumentRepository
{
    public VehicleDocumentRepository(AppDbContext context, IPaginationService paginationService)
        : base(context, paginationService)
    {
    }

    public Task<PaginationResult<VehicleDocument>> GetByVehicleIdPaginatedAsync(
        Guid vehicleId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query =
            GetQuery()
            .FilterByVehicleId(vehicleId)
            .OrderByDescending(d => d.CreatedAt);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }
}
