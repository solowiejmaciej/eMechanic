namespace eMechanic.Application.Repair.Features.Get.ById.ForWorkshop;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using Common.CQRS;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.Workshop)]
public sealed record GetWorkshopRepairByIdQuery(Guid RepairId) : IResultQuery<RepairResponse>;

public sealed class GetWorkshopRepairByIdQueryValidator : AbstractValidator<GetWorkshopRepairByIdQuery>
{
    public GetWorkshopRepairByIdQueryValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}

