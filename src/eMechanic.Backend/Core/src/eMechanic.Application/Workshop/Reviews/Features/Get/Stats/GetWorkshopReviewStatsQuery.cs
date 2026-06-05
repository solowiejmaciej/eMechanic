namespace eMechanic.Application.Workshop.Reviews.Features.Get.Stats;

using Common.Cache;
using Common.CQRS;
using FluentValidation;

[Cache(300, ECacheScope.Public)]
public sealed record GetWorkshopReviewStatsQuery(Guid WorkshopId) : IResultQuery<WorkshopReviewStatsResponse>;

public sealed class GetWorkshopReviewStatsQueryValidator : AbstractValidator<GetWorkshopReviewStatsQuery>
{
    public GetWorkshopReviewStatsQueryValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
    }
}
