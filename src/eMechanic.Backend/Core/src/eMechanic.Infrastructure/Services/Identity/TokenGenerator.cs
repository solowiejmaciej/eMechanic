namespace eMechanic.Infrastructure.Services.Identity;

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Identity;
using Application.Identity;
using Common.Helpers;
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
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identity.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, identity.Email!),
            new(JwtRegisteredClaimNames.Jti, GuidFactory.Create().ToString()),
            new("identityType", identity.Type.ToString())
        };

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

        return new TokenDTO(tokenString, expires);
    }

}
