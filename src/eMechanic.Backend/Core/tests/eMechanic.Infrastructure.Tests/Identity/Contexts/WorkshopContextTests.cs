namespace eMechanic.Infrastructure.Tests.Identity.Contexts;

using System.Security.Claims;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Identity.Contexts;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class WorkshopContextTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WorkshopContext _workshopContext;

    public WorkshopContextTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _workshopContext = new WorkshopContext(_httpContextAccessor);
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
        var isAuthenticated = _workshopContext.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_Should_ReturnFalse_WhenIdentityIsNotAuthenticated()
    {
        // Arrange
        SetupHttpContext(false);

        // Act
        var isAuthenticated = _workshopContext.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void IsAuthenticated_Should_ReturnFalse_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext)null!);

        // Act
        var isAuthenticated = _workshopContext.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void GetWorkshopId_Should_ReturnWorkshopId_WhenUserIsAuthenticatedAndClaimExists()
    {
        // Arrange
        var expectedWorkshopId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim(ClaimConstants.WORKSHOP_ID, expectedWorkshopId.ToString())
        };
        SetupHttpContext(true, claims);

        // Act
        var workshopId = _workshopContext.GetWorkshopId();

        // Assert
        workshopId.Should().Be(expectedWorkshopId);
    }

    [Fact]
    public void GetWorkshopId_Should_ThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
        SetupHttpContext(false);

        // Act
        Action act = () => _workshopContext.GetWorkshopId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Workshop is not authenticated.");
    }

    [Fact]
    public void GetWorkshopId_Should_ThrowUnauthorizedAccessException_WhenClaimIsMissing()
    {
        // Arrange
        // Użytkownik uwierzytelniony, ale bez claima WORKSHOP_ID
        SetupHttpContext(true, new List<Claim>());

        // Act
        Action act = () => _workshopContext.GetWorkshopId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("WorkshopId claim is missing or invalid.");
    }

    [Fact]
    public void GetWorkshopId_Should_ThrowUnauthorizedAccessException_WhenClaimIsInvalidGuid()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimConstants.WORKSHOP_ID, "to-nie-jest-guid")
        };
        SetupHttpContext(true, claims);

        // Act
        Action act = () => _workshopContext.GetWorkshopId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("WorkshopId claim is missing or invalid.");
    }

    [Fact]
    public void GetWorkshopId_Should_ThrowUnauthorizedAccessException_WhenClaimIsUserId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimConstants.USER_ID, Guid.NewGuid().ToString())
        };
        SetupHttpContext(true, claims);

        // Act
        Action act = () => _workshopContext.GetWorkshopId();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("WorkshopId claim is missing or invalid.");
    }
}
