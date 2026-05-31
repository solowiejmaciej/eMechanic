namespace eMechanic.Application.Repair.Features.Get.ById;

using Common.CQRS;
using FluentValidation;

public sealed record GetRepairByIdQuery(Guid RepairId) : IResultQuery<RepairResponse>;

public sealed class GetRepairByIdQueryValidator : AbstractValidator<GetRepairByIdQuery>
{
    public GetRepairByIdQueryValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}

