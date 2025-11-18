
namespace eMechanic.Application.RepairRequest.Features.Get.ForWorkshop;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record GetRepairRequestsForWorkshopQuery(PaginationParameters Pagination) : IResultQuery<PaginationResult<RepairRequestResponse>>;

public class GetRepairRequestsForWorkshopQueryValidator : AbstractValidator<GetRepairRequestsForWorkshopQuery>
{
    public GetRepairRequestsForWorkshopQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Pagination.PageSize).GreaterThanOrEqualTo(1);
    }
}
