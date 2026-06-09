namespace eMechanic.Application.Workshop.Document.Features.Get;

using eMechanic.Application.Storage.Dtos;
using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record GetWorkshopDocumentFileQuery(
    Guid WorkshopId,
    Guid DocumentId) : IResultQuery<FileDownloadResult>;

public class GetWorkshopDocumentFileQueryValidator : AbstractValidator<GetWorkshopDocumentFileQuery>
{
    public GetWorkshopDocumentFileQueryValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
        RuleFor(x => x.DocumentId).NotEmpty();
    }
}
