namespace eMechanic.API.Features.Workshop.Reviews.Request;

using Application.Workshop.Reviews.Features.Upsert;

public sealed record UpsertWorkshopReviewRequest(byte Rating, string? Comment)
{
    public UpsertWorkshopReviewCommand MapToCommand(Guid workshopId)
        => new(workshopId, Rating, Comment);
}

