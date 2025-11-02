namespace eMechanic.Infrastructure.Repositories;

using Application.Abstractions.Workshop;
using Base;
using Common.Result;
using DAL;
using Domain.Workshop;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Services;

internal sealed class WorkshopRepository : Repository<Workshop>, IWorkshopRepository
{
    public WorkshopRepository(AppDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
    }

    public Task<Workshop?> GetByIdentityIdAsync(Guid identityId)
    {
        return GetQuery()
            .FilterByIdentityId(identityId)
            .SingleOrDefaultAsync();
    }

    public Task<PaginationResult<Workshop>> GetPaginatedAsync(PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQuery();
        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }
}
