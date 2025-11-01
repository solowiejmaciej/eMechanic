namespace eMechanic.Application.Abstractions.Workshop;

using Common.Result;
using Domain.Workshop;
using Repositories;

public interface IWorkshopRepository : IRepository<Workshop>
{
    Task<Workshop?> GetByIdentityIdAsync(Guid identityId);
    Task<PaginationResult<Workshop>> GetPaginatedAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);
}
