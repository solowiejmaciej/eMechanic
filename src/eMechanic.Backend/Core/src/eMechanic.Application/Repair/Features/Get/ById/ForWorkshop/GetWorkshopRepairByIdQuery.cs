namespace eMechanic.Application.Repair.Features.Get.ById.ForWorkshop;

using Common.CQRS;
using FluentValidation;

public sealed record GetWorkshopRepairByIdQuery(Guid RepairId) : IResultQuery<RepairResponse>;

public sealed class GetWorkshopRepairByIdQueryValidator : AbstractValidator<GetWorkshopRepairByIdQuery>
{
    public GetWorkshopRepairByIdQueryValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}

