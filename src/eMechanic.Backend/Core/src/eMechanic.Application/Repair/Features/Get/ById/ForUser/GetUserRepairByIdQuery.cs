namespace eMechanic.Application.Repair.Features.Get.ById.ForUser;

using Common.CQRS;
using FluentValidation;

public sealed record GetUserRepairByIdQuery(Guid RepairId) : IResultQuery<RepairResponse>;

public sealed class GetUserRepairByIdQueryValidator : AbstractValidator<GetUserRepairByIdQuery>
{
    public GetUserRepairByIdQueryValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}

