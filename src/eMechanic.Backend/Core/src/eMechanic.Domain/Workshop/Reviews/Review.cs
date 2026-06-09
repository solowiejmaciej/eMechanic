namespace eMechanic.Domain.Workshop.Reviews;

using Common.Attributes;
using Common.DDD;
using Common.Result;
using DomainEvents;
using Shared.References.User;
using Shared.References.Workshop;
using ValueObjects;

public class Review : AggregateRoot, IUserReferenced, IWorkshopReference
{
    public Guid WorkshopId { get; private set; }
    public Guid UserId { get; private set; }

    [Searchable]
    public ReviewRating Rating { get; private set; }

    [Searchable]
    public ReviewComment? Comment { get; private set; }

    private Review()
    {
    }

    private Review(Guid workshopId, Guid userId, ReviewRating rating, ReviewComment? comment)
    {
        WorkshopId = workshopId;
        UserId = userId;
        Rating = rating;
        Comment = comment;

        RaiseDomainEvent(new WorkshopReviewCreatedDomainEvent(this));
    }

    public static Result<Review, Error> Create(Guid workshopId, Guid userId, byte ratingValue, string? comment)
    {
        if (workshopId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "WorkshopId cannot be empty.");
        }

        if (userId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "UserId cannot be empty.");
        }

        var ratingResult = ReviewRating.Create(ratingValue);
        if (ratingResult.HasError())
        {
            return ratingResult.Error!;
        }

        var commentResult = CreateCommentOrNull(comment);
        if (commentResult.HasError())
        {
            return commentResult.Error!;
        }

        return new Review(workshopId, userId, ratingResult.Value, commentResult.Value);
    }

    public Result<Success, Error> Update(byte ratingValue, string? comment)
    {
        var ratingResult = ReviewRating.Create(ratingValue);
        if (ratingResult.HasError())
        {
            return ratingResult.Error!;
        }

        var commentResult = CreateCommentOrNull(comment);
        if (commentResult.HasError())
        {
            return commentResult.Error!;
        }

        Rating = ratingResult.Value;
        Comment = commentResult.Value;
        RaiseDomainEvent(new WorkshopReviewUpdatedDomainEvent(this));

        return Result.Success;
    }

    public Result<Success, Error> Delete()
    {
        RaiseDomainEvent(new WorkshopReviewDeletedDomainEvent(this));
        return Result.Success;
    }

    private static Result<ReviewComment?, Error> CreateCommentOrNull(string? comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            return (ReviewComment?)null;
        }

        var commentResult = ReviewComment.Create(comment);
        if (commentResult.HasError())
        {
            return commentResult.Error!;
        }

        return commentResult.Value;
    }
}
