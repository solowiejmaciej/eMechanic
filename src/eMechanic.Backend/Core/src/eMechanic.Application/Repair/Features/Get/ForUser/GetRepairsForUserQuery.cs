namespace eMechanic.Application.Repair.Features.Get.ForUser;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using Common.CQRS;
using Common.Result;
using FluentValidation;
using eMechanic.Application.Repair.Features.Get;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetRepairsForUserQuery(PaginationParameters Pagination) : IResultQuery<PaginationResult<RepairListItemResponse>>;

public sealed class GetRepairsForUserQueryValidator : AbstractValidator<GetRepairsForUserQuery>
{
    public GetRepairsForUserQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Pagination.PageSize).GreaterThanOrEqualTo(1);
    }
}

