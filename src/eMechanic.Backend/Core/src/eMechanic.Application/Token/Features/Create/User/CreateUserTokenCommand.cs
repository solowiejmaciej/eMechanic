namespace eMechanic.Application.Token.Features.Create.User;

using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record CreateUserTokenCommand(
    string Email,
    string Password) : IResultCommand<CreateUserTokenResponse>;

public class CreateUserTokenValidator : AbstractValidator<CreateUserTokenCommand>
{
    public CreateUserTokenValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
