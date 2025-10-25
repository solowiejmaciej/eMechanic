namespace eMechanic.Application.Workshop.Login;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class LoginWorkshopHandler : IResultCommandHandler<LoginWorkshopCommand, LoginWorkshopResponse>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginWorkshopHandler(IAuthenticator authenticator, ITokenGenerator tokenGenerator)
    {
        _authenticator = authenticator;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<LoginWorkshopResponse, Error>> Handle(LoginWorkshopCommand request, CancellationToken cancellationToken)
    {
        var authResult = await _authenticator.AuthenticateAsync(
            request.Email,
            request.Password,
            EIdentityType.Workshop);

        if (authResult.HasError())
        {
            return authResult.Error!;
        }

        if(authResult.Value == null)
        {
            return new Error(EErrorCode.InternalServerError, "Authentication failed unexpectedly.");
        }

        var token = _tokenGenerator.GenerateToken(authResult.Value);

        return new LoginWorkshopResponse(token.AccessToken, token.ExpiresAt, authResult.Value!.Id);
    }
}
