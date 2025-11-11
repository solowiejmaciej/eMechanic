namespace eMechanic.Application.Token.Features.Create.User;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class CreateUserTokenCommandHandler : IResultCommandHandler<CreateUserTokenCommand, CreateUserTokenResponse>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public CreateUserTokenCommandHandler(
        IAuthenticator authenticator,
        ITokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _authenticator = authenticator;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<CreateUserTokenResponse, Error>> Handle(CreateUserTokenCommand request, CancellationToken cancellationToken)
    {
        var authResult = await _authenticator.AuthenticateAsync(
            request.Email,
            request.Password,
            EIdentityType.User);

        if (authResult.HasError())
        {
            return authResult.Error!;
        }

        if(authResult.Value == null)
        {
            return new Error(EErrorCode.InternalServerError, "Authentication failed unexpectedly.");
        }

        var token = _tokenGenerator.GenerateToken(authResult.Value);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(authResult.Value.IdentityId, token.Jti, cancellationToken);

        return new CreateUserTokenResponse(token.AccessToken, token.ExpiresAt, authResult.Value.DomainEntityId, refreshToken);
    }
}
