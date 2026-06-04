namespace eMechanic.Application.Workshop.Reviews.Features.Get;

public sealed record WorkshopReviewResponse(
    Guid Id,
    Guid WorkshopId,
    Guid UserId,
    byte Rating,
    string? Comment,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

