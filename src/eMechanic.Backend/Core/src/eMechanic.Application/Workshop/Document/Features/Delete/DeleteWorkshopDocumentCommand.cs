namespace eMechanic.Application.Workshop.Document.Features.Delete;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using Common.Result;
using FluentValidation;
using Get;

[InvalidatesCache(typeof(GetWorkshopDocumentsQuery))]
public sealed record DeleteWorkshopDocumentCommand(Guid DocumentId) : IResultCommand<Success>;

public class DeleteWorkshopDocumentCommandValidator : AbstractValidator<DeleteWorkshopDocumentCommand>
{
    public DeleteWorkshopDocumentCommandValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID must be provided.");
    }
}
