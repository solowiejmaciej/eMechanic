namespace eMechanic.Application.Workshop.Reviews.Features.Get.All;

using Common.Cache;
using Common.CQRS;
using Common.Result;
using FluentValidation;

[Cache(300, ECacheScope.Public)]
public sealed record GetWorkshopReviewsQuery(Guid WorkshopId, PaginationParameters PaginationParameters)
    : IResultQuery<PaginationResult<WorkshopReviewResponse>>;

public sealed class GetWorkshopReviewsQueryValidator : AbstractValidator<GetWorkshopReviewsQuery>
{
    public GetWorkshopReviewsQueryValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
        RuleFor(x => x.PaginationParameters.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PaginationParameters.PageSize).GreaterThanOrEqualTo(1);
    }
}
