namespace eMechanic.Infrastructure.Services.Identity;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Identity;
using Application.Identity;
using Application.Users.Repositories;
using Application.Workshop.Repositories;
using Common.Result;
using Common.Result.Fields;
using DAL;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IdentityAppDbContext _identityDbContext;
    private readonly IConfiguration _configuration;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly UserManager<Identity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly TokenValidationParameters _tokenValidationParams;

    public RefreshTokenService(
        IdentityAppDbContext identityDbContext,
        IConfiguration configuration,
        ITokenGenerator tokenGenerator,
        UserManager<Identity> userManager,
        IUserRepository userRepository,
        IWorkshopRepository workshopRepository,
        ITransactionalExecutor transactionalExecutor,
        ILogger<RefreshTokenService> logger)
    {
        _identityDbContext = identityDbContext;
        _configuration = configuration;
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
        _userRepository = userRepository;
        _workshopRepository = workshopRepository;
        _transactionalExecutor = transactionalExecutor;
        _logger = logger;

        _tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Authentication:JwtBearer:Issuer"],
            ValidAudience = _configuration["Authentication:JwtBearer:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JwtBearer:Key"]!))
        };
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid identityId, Guid jti, CancellationToken cancellationToken)
    {
        var expiryDays = _configuration.GetValue<int>("Authentication:JwtBearer:RefreshTokenExpiryInDays", 7);
        var expiryDate = DateTime.UtcNow.AddDays(expiryDays);

        var refreshToken = RefreshToken.Create(jti, expiryDate, identityId);

        await _identityDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _identityDbContext.SaveChangesAsync(cancellationToken);

        return refreshToken.Token;
    }

    public async Task<Result<RefreshTokenDTO, Error>> ValidateAndRotateRefreshTokenAsync(
        string expiredAccessToken, string refreshToken, CancellationToken ct)
    {
        ClaimsPrincipal? principal;
        try
        {
            principal = GetPrincipalFromExpiredToken(expiredAccessToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Error.Validation(EField.General, "Invalid token.");
        }

        var identityIdStr = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var jtiStr = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);

        if (!Guid.TryParse(identityIdStr, out var identityId) || !Guid.TryParse(jtiStr, out var jti))
            return Error.Validation(EField.General, "Invalid token claims (Id or Jti).");

        RefreshTokenDTO? responseData = null;

        try
        {
            await _transactionalExecutor.ExecuteAsync(async () =>
            {
                var dbToken = await _identityDbContext.RefreshTokens
                    .SingleOrDefaultAsync(rt => rt.Token == refreshToken, ct);

                if (dbToken == null)
                    throw new InvalidOperationException("Invalid refresh token.");
                if (!dbToken.IsActive)
                    throw new InvalidOperationException("Refresh token is not active (expired or already used).");
                if (dbToken.IdentityId != identityId)
                    throw new InvalidOperationException("Refresh token identity mismatch.");
                if (dbToken.Jti != jti)
                    throw new InvalidOperationException("Refresh token JTI mismatch (token reuse suspected).");

                dbToken.SetUsed();

                var identityUser = await _userManager.FindByIdAsync(identityId.ToString());
                if (identityUser == null)
                    throw new InvalidOperationException("Identity user not found.");

                var domainEntityId = await GetDomainEntityIdAsync(identityUser.Id, identityUser.Type, ct);
                if (domainEntityId == null)
                    throw new InvalidOperationException("Domain entity not found for identity.");

                var authIdentity = new AuthenticatedIdentity(identityUser.Id, domainEntityId.Value, identityUser.Email!,
                    identityUser.Type);
                var newAccessTokenDto = _tokenGenerator.GenerateToken(authIdentity);

                var newJti = GetJtiFromToken(newAccessTokenDto.AccessToken);
                if (newJti == Guid.Empty)
                    throw new InvalidOperationException("Failed to generate JTI for new access token.");

                var newRefreshToken = await GenerateRefreshTokenAsync(identityUser.Id, newJti, ct);

                await _identityDbContext.SaveChangesAsync(ct);

                responseData = new RefreshTokenDTO(
                    newAccessTokenDto.AccessToken,
                    newAccessTokenDto.ExpiresAt,
                    domainEntityId.Value,
                    newRefreshToken,
                    identityUser.Type
                );

            }, ct);

            return responseData!;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return new Error(EErrorCode.UnauthorizedError, "Unable to authorize identity");
        }

    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };

        var principal = tokenHandler.ValidateToken(token, _tokenValidationParams, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token or algorithm.");
        }

        return principal;
    }

    private async Task<Guid?> GetDomainEntityIdAsync(Guid identityId, EIdentityType type, CancellationToken ct)
    {
        return type switch
        {
            EIdentityType.User => (await _userRepository.GetByIdentityIdAsync(identityId))?.Id,
            EIdentityType.Workshop => (await _workshopRepository.GetByIdentityIdAsync(identityId))?.Id,
            _ => null
        };
    }

    private Guid GetJtiFromToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        handler.MapInboundClaims = false;

        if (handler.ReadToken(accessToken) is not JwtSecurityToken jwtToken)
        {
            return Guid.Empty;
        }

        var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

        return Guid.TryParse(jtiClaim?.Value, out var jti) ? jti : Guid.Empty;
    }
}
