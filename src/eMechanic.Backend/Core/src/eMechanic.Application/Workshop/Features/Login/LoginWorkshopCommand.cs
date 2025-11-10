namespace eMechanic.Application.Workshop.Features.Login;

using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record LoginWorkshopCommand(
    string Email,
    string Password) : IResultCommand<LoginWorkshopResponse>;

public class LoginWorkshopCommandValidator : AbstractValidator<LoginWorkshopCommand>
{
    public LoginWorkshopCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
