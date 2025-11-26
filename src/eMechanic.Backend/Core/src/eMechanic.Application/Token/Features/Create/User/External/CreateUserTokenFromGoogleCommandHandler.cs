namespace eMechanic.Application.Token.Features.Create.User.External;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

internal sealed class CreateUserTokenFromGoogleCommandHandler : IResultCommandHandler<CreateUserTokenFromGoogleCommand, CreateUserTokenResponse>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public CreateUserTokenFromGoogleCommandHandler(
        IGoogleAuthService googleAuthService,
        ITokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _googleAuthService = googleAuthService;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<CreateUserTokenResponse, Error>> Handle(CreateUserTokenFromGoogleCommand request,
        CancellationToken cancellationToken)
    {
        var authenticatedIdentity = await _googleAuthService.LoginAsync(request.IdToken, cancellationToken);

        if (authenticatedIdentity.HasError())
        {
            return authenticatedIdentity.Error!;
        }

        if (authenticatedIdentity.Value == null)
        {
            return new Error(EErrorCode.InternalServerError, "Google authentication failed unexpectedly.");
        }

        var token = _tokenGenerator.GenerateToken(authenticatedIdentity.Value);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(authenticatedIdentity.Value.IdentityId, token.Jti, cancellationToken);

        return new CreateUserTokenResponse(token.AccessToken, token.ExpiresAt, authenticatedIdentity.Value.DomainEntityId, refreshToken);
    }
}
