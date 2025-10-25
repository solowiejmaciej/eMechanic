using System.IdentityModel.Tokens.Jwt;
using eMechanic.Application.Identity;
using eMechanic.Infrastructure.Services.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace eMechanic.Infrastructure.Tests.Services.Identity;

using Application.Abstractions.Identity;

public class TokenGeneratorTests
{
    private readonly IConfiguration _configuration;
    private readonly TokenGenerator _tokenGenerator;

    private const string TEST_SECRET_KEY = "SuperTajnyKluczTestowy1234567890!@#";
    private const string TEST_ISSUER = "https://test.issuer.com";
    private const string TEST_AUDIENCE = "https://test.audience.com";
    private const string TEST_EXPIRY_MINUTES = "60";

    public TokenGeneratorTests()
    {
        _configuration = Substitute.For<IConfiguration>();

        _configuration["Authentication:JwtBearer:Key"].Returns(TEST_SECRET_KEY);
        _configuration["Authentication:JwtBearer:ExpiryInMinutes"].Returns(TEST_EXPIRY_MINUTES);
        _configuration["Authentication:JwtBearer:Issuer"].Returns(TEST_ISSUER);
        _configuration["Authentication:JwtBearer:Audience"].Returns(TEST_AUDIENCE);

        _tokenGenerator = new TokenGenerator(_configuration);
    }

    [Fact]
    public void GenerateToken_Should_CreateTokenWithCorrectClaimsAndExpiry()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var domainEntityId = Guid.NewGuid();

        var identity = new AuthenticatedIdentity(
            identityId,
            domainEntityId,
            "test@user.com",
            EIdentityType.User);

        var expectedExpiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(TEST_EXPIRY_MINUTES, System.Globalization.CultureInfo.InvariantCulture));

        // Act
        var tokenDto = _tokenGenerator.GenerateToken(identity);

        // Assert
        Assert.NotNull(tokenDto);
        Assert.False(string.IsNullOrWhiteSpace(tokenDto.AccessToken));

        Assert.True(tokenDto.ExpiresAt > expectedExpiry.AddSeconds(-10));
        Assert.True(tokenDto.ExpiresAt < expectedExpiry.AddSeconds(10));

        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(tokenDto.AccessToken);

        var subClaim = decodedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        var emailClaim = decodedToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
        var typeClaim = decodedToken.Claims.First(c => c.Type == "identityType").Value;

        Assert.Equal(identityId.ToString(), subClaim);
        Assert.Equal(identity.Email, emailClaim);
        Assert.Equal(identity.Type.ToString(), typeClaim);
    }
}
