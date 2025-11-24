namespace eMechanic.Application.Workshop.Document.Features.Get;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record GetWorkshopDocumentsQuery(
    Guid WorkshopId,
    PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<WorkshopDocumentResponse>>;

public class GetVehicleDocumentsQueryValidator : AbstractValidator<GetWorkshopDocumentsQuery>
{
    public GetVehicleDocumentsQueryValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
        RuleFor(x => x.PaginationParameters.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PaginationParameters.PageSize).GreaterThanOrEqualTo(1);
    }
}
