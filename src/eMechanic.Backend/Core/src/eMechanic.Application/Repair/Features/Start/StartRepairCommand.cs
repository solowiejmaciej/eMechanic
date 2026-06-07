namespace eMechanic.Application.Repair.Features.Start;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using Common.Result;
using FluentValidation;
using Get.ById.ForUser;
using Get.ById.ForWorkshop;
using Get.ForUser;
using Get.ForWorkshop;

[InvalidatesCache(
    typeof(GetRepairsForUserQuery),
    typeof(GetRepairsForWorkshopQuery),
    typeof(GetUserRepairByIdQuery),
    typeof(GetWorkshopRepairByIdQuery))]
public sealed record StartRepairCommand(Guid RepairId) : IResultCommand<Success>;

public sealed class StartRepairCommandValidator : AbstractValidator<StartRepairCommand>
{
    public StartRepairCommandValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}

