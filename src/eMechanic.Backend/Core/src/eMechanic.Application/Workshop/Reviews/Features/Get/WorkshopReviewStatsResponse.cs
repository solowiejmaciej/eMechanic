namespace eMechanic.Application.Workshop.Reviews.Features.Get;

public sealed record WorkshopReviewStatsResponse(Guid WorkshopId, decimal AverageRating, int TotalReviews);

