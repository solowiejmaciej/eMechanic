namespace eMechanic.Infrastructure.Tests.Identity.Contexts;

using System.Security.Claims;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Identity.Contexts;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class UserContextTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserContext _userContext;

    public UserContextTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _userContext = new UserContext(_httpContextAccessor);
    }

    private void SetupHttpContext(bool isAuthenticated, IEnumerable<Claim>? claims = null)
    {
        var httpContext = new DefaultHttpContext();
        if (isAuthenticated)
        {
            var identity = new ClaimsIdentity(claims, "TestAuth");
            httpContext.User = new ClaimsPrincipal(identity);
        }
        else
        {
            var identity = new ClaimsIdentity(claims);
            httpContext.User = new ClaimsPrincipal(identity);
        }
        _httpContextAccessor.HttpContext.Returns(httpContext);
    }

    [Fact]
    public void IsAuthenticated_Should_ReturnTrue_WhenIdentityIsAuthenticated()
    {
        // Arrange
        SetupHttpContext(true);

        // Act
        var isAuthenticated = _userContext.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_Should_ReturnFalse_WhenIdentityIsNotAuthenticated()
    {
        // Arrange
        SetupHttpContext(false);

        // Act
        var isAuthenticated = _userContext.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void IsAuthenticated_Should_ReturnFalse_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext)null!);

        // Act
        var isAuthenticated = _userContext.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void GetUserId_Should_ReturnUserId_WhenUserIsAuthenticatedAndClaimExists()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim(ClaimConstants.USER_ID, expectedUserId.ToString())
        };
        SetupHttpContext(true, claims);

        // Act
        var userId = _userContext.GetUserId();

        // Assert
        userId.Should().Be(expectedUserId);
    }

    [Fact]
    public void GetUserId_Should_ThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
        SetupHttpContext(false);

        // Act
        Action act = () => _userContext.GetUserId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("User is not authenticated.");
    }

    [Fact]
    public void GetUserId_Should_ThrowUnauthorizedAccessException_WhenClaimIsMissing()
    {
        // Arrange
        SetupHttpContext(true, new List<Claim>());

        // Act
        Action act = () => _userContext.GetUserId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("User ID claim is missing or invalid.");
    }

    [Fact]
    public void GetUserId_Should_ThrowUnauthorizedAccessException_WhenClaimIsInvalidGuid()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimConstants.USER_ID, "to-nie-jest-guid")
        };
        SetupHttpContext(true, claims);

        // Act
        Action act = () => _userContext.GetUserId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("User ID claim is missing or invalid.");
    }

    [Fact]
    public void GetUserId_Should_ThrowUnauthorizedAccessException_WhenClaimIsWorkshopId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimConstants.WORKSHOP_ID, Guid.NewGuid().ToString())
        };
        SetupHttpContext(true, claims);

        // Act
        Action act = () => _userContext.GetUserId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("User ID claim is missing or invalid.");
    }
}
