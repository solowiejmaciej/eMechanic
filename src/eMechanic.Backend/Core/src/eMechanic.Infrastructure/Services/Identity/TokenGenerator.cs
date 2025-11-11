namespace eMechanic.Infrastructure.Services.Identity;

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Identity;
using Application.Identity;
using Common.Helpers;
using Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

public sealed class TokenGenerator : ITokenGenerator
{
    private readonly IConfiguration _configuration;

    public TokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenDTO GenerateToken(AuthenticatedIdentity identity)
    {
        var jti = GuidFactory.Create();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identity.IdentityId.ToString()),
            new(JwtRegisteredClaimNames.Email, identity.Email!),
            new(JwtRegisteredClaimNames.Jti, jti.ToString()),
            new(ClaimConstants.IDENTITY_TYPE, identity.Type.ToString())
        };

        switch (identity.Type)
        {
            case EIdentityType.User:
                claims.Add(new Claim(ClaimConstants.USER_ID, identity.DomainEntityId.ToString()));
                break;
            case EIdentityType.Workshop:
                claims.Add(new Claim(ClaimConstants.WORKSHOP_ID, identity.DomainEntityId.ToString()));
                break;
        }
        var keyString = _configuration["Authentication:JwtBearer:Key"];
        var expiresMinutesString = _configuration["Authentication:JwtBearer:ExpiryInMinutes"];
        var issuer = _configuration["Authentication:JwtBearer:Issuer"];
        var audience = _configuration["Authentication:JwtBearer:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiresMinutesString, CultureInfo.InvariantCulture));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenDTO(tokenString, expires, jti);
    }

}
