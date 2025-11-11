namespace eMechanic.Application.Token.Features.Create.Workshop;

using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record CreateWorkshopTokenCommand(
    string Email,
    string Password) : IResultCommand<CreateWorkshopTokenResponse>;

public class CreateWorkshopTokenCommandValidator : AbstractValidator<CreateWorkshopTokenCommand>
{
    public CreateWorkshopTokenCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
