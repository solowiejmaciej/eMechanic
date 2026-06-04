namespace eMechanic.Application.Repair.Features.Get.ForWorkshop;

using Common.CQRS;
using Common.Result;
using FluentValidation;
using eMechanic.Application.Repair.Features.Get;

public sealed record GetRepairsForWorkshopQuery(PaginationParameters Pagination) : IResultQuery<PaginationResult<RepairListItemResponse>>;

public sealed class GetRepairsForWorkshopQueryValidator : AbstractValidator<GetRepairsForWorkshopQuery>
{
    public GetRepairsForWorkshopQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Pagination.PageSize).GreaterThanOrEqualTo(1);
    }
}

