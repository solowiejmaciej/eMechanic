namespace eMechanic.Infrastructure.Repositories;

using Application.Workshop.Reviews.Repositories;
using Base;
using Common.Result;
using DAL;
using Domain.Workshop.Reviews;
using Microsoft.EntityFrameworkCore;
using Repositories.Extensions;
using Services;

internal sealed class WorkshopReviewRepository : Repository<Review>, IWorkshopReviewRepository
{
    public WorkshopReviewRepository(AppDbContext context, IPaginationService paginationService)
        : base(context, paginationService)
    {
    }

    public Task<Review?> GetForWorkshopByUserAsTrackingAsync(Guid workshopId, Guid userId, CancellationToken cancellationToken)
    {
        return GetQuery()
            .FilterByUserId(userId)
            .FilterByWorkshopId(workshopId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<PaginationResult<Review>> GetForWorkshopPaginatedAsync(
        Guid workshopId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQuery()
            .AsNoTracking()
            .FilterByWorkshopId(workshopId)
            .OrderByDescending(x => x.CreatedAt);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }

    public async Task<WorkshopReviewStatsProjection> GetStatsForWorkshopAsync(Guid workshopId, CancellationToken cancellationToken)
    {
        var ratings = await GetQuery()
            .AsNoTracking()
            .FilterByWorkshopId(workshopId)
            .Select(x => x.Rating)
            .ToListAsync(cancellationToken);

        if (ratings.Count == 0)
        {
            return new WorkshopReviewStatsProjection(workshopId, 0, 0);
        }

        var averageRating = ratings.Average(x => (decimal)x.Value);

        return new WorkshopReviewStatsProjection(workshopId, averageRating, ratings.Count);
    }
}
