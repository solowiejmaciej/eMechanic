namespace eMechanic.Application.Workshop.Reviews.Features.Delete;

using Common.CQRS;
using FluentValidation;

public sealed record DeleteWorkshopReviewCommand(Guid WorkshopId) : IResultCommand<bool>;

public sealed class DeleteWorkshopReviewCommandValidator : AbstractValidator<DeleteWorkshopReviewCommand>
{
    public DeleteWorkshopReviewCommandValidator()
    {
        RuleFor(x => x.WorkshopId).NotEmpty();
    }
}

