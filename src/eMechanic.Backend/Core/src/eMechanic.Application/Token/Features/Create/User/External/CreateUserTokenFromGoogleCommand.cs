namespace eMechanic.Application.Token.Features.Create.User.External;

using eMechanic.Application.Identity;
using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record CreateUserTokenFromGoogleCommand(string IdToken) : IResultCommand<CreateUserTokenResponse>;

public class CreateUserTokenFromGoogleCommandValidator : AbstractValidator<CreateUserTokenFromGoogleCommand>
{
    public CreateUserTokenFromGoogleCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("IdToken is required.");
    }
}
