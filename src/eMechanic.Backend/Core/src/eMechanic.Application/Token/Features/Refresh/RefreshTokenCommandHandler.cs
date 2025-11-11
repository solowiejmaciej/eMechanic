namespace eMechanic.Application.Token.Features.Refresh;

using Abstractions.Identity;
using Common.CQRS;
using Common.Result;

public class RefreshTokenCommandHandler : IResultCommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenCommandHandler(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<RefreshTokenResponse, Error>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _refreshTokenService.ValidateAndRotateRefreshTokenAsync(request.AccessToken, request.RefreshToken, cancellationToken);
        if (result.HasError())
        {
            return result.Error!;
        }

        var response = new RefreshTokenResponse(result.Value!.NewAccessToken, result.Value.NewExpiresAt, result.Value.NewRefreshToken);

        return response;
    }
}
