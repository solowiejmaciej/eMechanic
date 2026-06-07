
namespace eMechanic.Application.RepairRequest.Features.Get.ForWorkshop;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using Common.CQRS;
using Common.Result;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.Workshop)]
public sealed record GetRepairRequestsForWorkshopQuery(PaginationParameters Pagination) : IResultQuery<PaginationResult<RepairRequestResponse>>;

public class GetRepairRequestsForWorkshopQueryValidator : AbstractValidator<GetRepairRequestsForWorkshopQuery>
{
    public GetRepairRequestsForWorkshopQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Pagination.PageSize).GreaterThanOrEqualTo(1);
    }
}
