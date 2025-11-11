namespace eMechanic.Application.Abstractions.Identity;

using Application.Identity;
using Common.Result;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshTokenAsync(Guid identityId, Guid jti, CancellationToken cancellationToken);

    Task<Result<RefreshTokenDTO, Error>> ValidateAndRotateRefreshTokenAsync(
        string expiredAccessToken,
        string refreshToken,
        CancellationToken ct);
}
