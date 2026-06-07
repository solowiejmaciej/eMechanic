namespace eMechanic.Application.Workshop.Workshop.Features.Get.All;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.Public)]
public sealed record GetWorkshopsQuery(PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<WorkshopResponse>>;

public class GetWorkshopsQueryValidator : AbstractValidator<GetWorkshopsQuery>
{
    public GetWorkshopsQueryValidator()
    {
        RuleFor(x => x.PaginationParameters.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PaginationParameters.PageSize).GreaterThanOrEqualTo(1);
    }
}
