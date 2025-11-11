namespace eMechanic.Application.Token.Features.Refresh;

using Common.CQRS;
using FluentValidation;

public sealed record RefreshTokenCommand(
    string RefreshToken,
    string AccessToken) : IResultCommand<RefreshTokenResponse>;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}
