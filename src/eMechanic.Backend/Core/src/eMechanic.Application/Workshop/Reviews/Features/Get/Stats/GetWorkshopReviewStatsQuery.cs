namespace eMechanic.Application.Workshop.Reviews.Features.Get.Stats;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using Common.CQRS;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.Public)]
public sealed record GetWorkshopReviewStatsQuery(Guid WorkshopId) : IResultQuery<WorkshopReviewStatsResponse>;

public sealed class GetWorkshopReviewStatsQueryValidator : AbstractValidator<GetWorkshopReviewStatsQuery>
{
    public GetWorkshopReviewStatsQueryValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
    }
}
