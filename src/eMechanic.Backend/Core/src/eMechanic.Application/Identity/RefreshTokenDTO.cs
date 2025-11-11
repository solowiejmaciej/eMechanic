namespace eMechanic.Application.Identity;

public record RefreshTokenDTO(
    string NewAccessToken,
    DateTime NewExpiresAt,
    Guid DomainEntityId,
    string NewRefreshToken,
    EIdentityType IdentityType);
