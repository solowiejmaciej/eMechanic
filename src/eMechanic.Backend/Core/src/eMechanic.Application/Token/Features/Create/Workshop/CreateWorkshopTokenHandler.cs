namespace eMechanic.Application.Token.Features.Create.Workshop;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class CreateWorkshopTokenHandler : IResultCommandHandler<CreateWorkshopTokenCommand, CreateWorkshopTokenResponse>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public CreateWorkshopTokenHandler(
        IAuthenticator authenticator,
        ITokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _authenticator = authenticator;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<CreateWorkshopTokenResponse, Error>> Handle(CreateWorkshopTokenCommand request, CancellationToken cancellationToken)
    {
        var authResult = await _authenticator.AuthenticateAsync(
            request.Email,
            request.Password,
            EIdentityType.Workshop);

        if (authResult.HasError())
        {
            return authResult.Error!;
        }

        if (authResult.Value == null)
        {
            return new Error(EErrorCode.InternalServerError, "Authentication failed unexpectedly.");
        }

        var token = _tokenGenerator.GenerateToken(authResult.Value);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(authResult.Value.IdentityId, token.Jti, cancellationToken);

        return new CreateWorkshopTokenResponse(token.AccessToken, token.ExpiresAt, authResult.Value!.DomainEntityId, refreshToken);
    }
}
