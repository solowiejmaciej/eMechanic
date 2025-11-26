namespace eMechanic.API.Features.Tokens.Create.External.Request;

using Application.Token.Features.Create.User.External;

public sealed record CreateGoogleTokenRequest(string IdToken)
{
    public CreateUserTokenFromGoogleCommand MapToCommand() =>
        new(IdToken);
}
