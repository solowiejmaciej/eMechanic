namespace eMechanic.Application.Users.Login;

using Abstractions.Identity;
using Common.CQRS;
using FluentValidation;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IResultCommand<LoginUserResponse>;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
