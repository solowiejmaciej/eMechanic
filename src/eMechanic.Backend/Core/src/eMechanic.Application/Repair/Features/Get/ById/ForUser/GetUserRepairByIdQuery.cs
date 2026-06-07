namespace eMechanic.Application.Repair.Features.Get.ById.ForUser;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using Common.CQRS;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetUserRepairByIdQuery(Guid RepairId) : IResultQuery<RepairResponse>;

public sealed class GetUserRepairByIdQueryValidator : AbstractValidator<GetUserRepairByIdQuery>
{
    public GetUserRepairByIdQueryValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}

