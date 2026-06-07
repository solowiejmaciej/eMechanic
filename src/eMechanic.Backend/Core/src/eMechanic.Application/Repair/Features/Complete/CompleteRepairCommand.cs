namespace eMechanic.Application.Repair.Features.Complete;

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
public sealed record CompleteRepairCommand(Guid RepairId, decimal Amount, string Currency) : IResultCommand<Success>;

public sealed class CompleteRepairCommandValidator : AbstractValidator<CompleteRepairCommand>
{
    public CompleteRepairCommandValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

