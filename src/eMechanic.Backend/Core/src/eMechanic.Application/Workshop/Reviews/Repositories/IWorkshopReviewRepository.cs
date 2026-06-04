namespace eMechanic.Application.Workshop.Reviews.Repositories;

using Abstractions.Repositories;
using Common.Result;
using Domain.Workshop.Reviews;

public interface IWorkshopReviewRepository : IRepository<Review>
{
    Task<Review?> GetForWorkshopByUserAsTrackingAsync(Guid workshopId, Guid userId, CancellationToken cancellationToken);
    Task<PaginationResult<Review>> GetForWorkshopPaginatedAsync(Guid workshopId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
    Task<WorkshopReviewStatsProjection> GetStatsForWorkshopAsync(Guid workshopId, CancellationToken cancellationToken);
}

