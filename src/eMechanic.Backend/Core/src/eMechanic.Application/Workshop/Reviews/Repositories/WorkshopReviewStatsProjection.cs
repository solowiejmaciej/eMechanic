namespace eMechanic.Application.Workshop.Reviews.Repositories;

public sealed record WorkshopReviewStatsProjection(Guid WorkshopId, decimal AverageRating, int TotalReviews);

