namespace eMechanic.API.Features.Shared;

public sealed record TokenRefreshRequest(string AccessToken, string RefreshToken);
