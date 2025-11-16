namespace eMechanic.Application.VehicleDocument.Repositories;

using Abstractions.Repositories;
using Common.Result;
using Domain.VehicleDocument;

public interface IVehicleDocumentRepository : IRepository<VehicleDocument>
{
    Task<PaginationResult<VehicleDocument>> GetByVehicleIdPaginatedAsync(
        Guid vehicleId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken);
}
