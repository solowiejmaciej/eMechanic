namespace eMechanic.Application.Token.Features.Refresh;

public sealed record RefreshTokenResponse(string Token, DateTime ExpiresAtUtc, string RefreshToken);
