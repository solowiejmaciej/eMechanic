namespace eMechanic.Application.Token.Features.Create.User;

public sealed record CreateUserTokenResponse(string Token, DateTime ExpiresAtUtc, Guid UserId, string RefreshToken);
