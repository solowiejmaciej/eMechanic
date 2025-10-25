namespace eMechanic.Application.Users.Login;

using Abstractions.Identity;
using Common.CQRS;
using Common.Result;

public class LoginUserHandler : IResultCommandHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginUserHandler(IAuthenticator authenticator, ITokenGenerator tokenGenerator)
    {
        _authenticator = authenticator;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<LoginUserResponse, Error>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
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

        return new LoginUserResponse(token.AccessToken, token.ExpiresAt, authResult.Value.Id);
    }
}
