namespace eMechanic.Application.Vehicle.Document.Repositories;

using Domain.Vehicle.Documents;
using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Common.Result;

public interface IVehicleDocumentRepository : IRepository<VehicleDocument>
{
    Task<PaginationResult<VehicleDocument>> GetByVehicleIdPaginatedAsync(
        Guid vehicleId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken);
}
