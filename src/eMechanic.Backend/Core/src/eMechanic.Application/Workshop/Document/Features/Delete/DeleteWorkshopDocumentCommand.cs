namespace eMechanic.Application.Workshop.Document.Features.Delete;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record DeleteWorkshopDocumentCommand(Guid DocumentId) : IResultCommand<Success>;

public class DeleteWorkshopDocumentCommandValidator : AbstractValidator<DeleteWorkshopDocumentCommand>
{
    public DeleteWorkshopDocumentCommandValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID must be provided.");
    }
}
