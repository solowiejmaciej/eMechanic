namespace eMechanic.API.Features.Tokens.Refresh.Request;

using Application.Token.Features.Refresh;

public sealed record RefreshTokenRequest(string RefreshToken, string AccessToken)
{
    public RefreshTokenCommand MapToCommand() => new(RefreshToken, AccessToken);
}
