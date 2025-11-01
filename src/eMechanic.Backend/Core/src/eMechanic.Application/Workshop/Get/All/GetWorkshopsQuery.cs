namespace eMechanic.Application.Workshop.Get.All;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record GetWorkshopsQuery(PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<WorkshopResponse>>;

public class GetWorkshopsQueryValidator : AbstractValidator<GetWorkshopsQuery>
{
    public GetWorkshopsQueryValidator()
    {
        RuleFor(x => x.PaginationParameters.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PaginationParameters.PageSize) .GreaterThanOrEqualTo(1);
    }
}
