namespace eMechanic.Infrastructure.Services;

using Common.Result;
using Microsoft.EntityFrameworkCore;

public class PaginationService : IPaginationService
{
    public async Task<PaginationResult<TEntity>>
        GetPaginatedResultAsync<TEntity>(IQueryable<TEntity> query, IPaginationParameters paginationParameters,
            CancellationToken cancellationToken)
    {
        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginationResult<TEntity>(items, count, paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }
}

public interface IPaginationService
{
    Task<PaginationResult<TEntity>> GetPaginatedResultAsync<TEntity>(
        IQueryable<TEntity> query,
        IPaginationParameters paginationParameters,
        CancellationToken cancellationToken);
}
