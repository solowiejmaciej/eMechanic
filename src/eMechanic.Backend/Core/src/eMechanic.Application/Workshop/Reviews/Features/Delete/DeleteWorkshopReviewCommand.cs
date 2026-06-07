namespace eMechanic.Application.Workshop.Reviews.Features.Delete;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using FluentValidation;
using Get.All;
using Get.Stats;

[InvalidatesCache(typeof(GetWorkshopReviewsQuery), typeof(GetWorkshopReviewStatsQuery))]
public sealed record DeleteWorkshopReviewCommand(Guid WorkshopId) : IResultCommand<bool>;

public sealed class DeleteWorkshopReviewCommandValidator : AbstractValidator<DeleteWorkshopReviewCommand>
{
    public DeleteWorkshopReviewCommandValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
    }
}

