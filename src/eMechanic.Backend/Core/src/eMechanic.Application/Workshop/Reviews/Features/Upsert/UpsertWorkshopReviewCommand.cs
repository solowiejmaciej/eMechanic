namespace eMechanic.Application.Workshop.Reviews.Features.Upsert;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using FluentValidation;
using Get.All;
using Get.Stats;

[InvalidatesCache(typeof(GetWorkshopReviewsQuery), typeof(GetWorkshopReviewStatsQuery))]
public sealed record UpsertWorkshopReviewCommand(Guid WorkshopId, byte Rating, string? Comment) : IResultCommand<Guid>;

public sealed class UpsertWorkshopReviewCommandValidator : AbstractValidator<UpsertWorkshopReviewCommand>
{
    public UpsertWorkshopReviewCommandValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween((byte)1, (byte)5);
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}

