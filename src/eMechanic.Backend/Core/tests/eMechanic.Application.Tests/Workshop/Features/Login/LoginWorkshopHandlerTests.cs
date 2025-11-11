namespace eMechanic.Application.Tests.Workshop.Features.Login;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Application.Workshop.Features.Login;
using eMechanic.Common.Result;
using eMechanic.Common.Result.Fields;
using FluentAssertions;
using NSubstitute;

public class LoginWorkshopHandlerTests
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly LoginWorkshopHandler _handler;

    public LoginWorkshopHandlerTests()
    {
        _authenticator = Substitute.For<IAuthenticator>();
        _tokenGenerator = Substitute.For<ITokenGenerator>();
        _refreshTokenService = Substitute.For<IRefreshTokenService>();

        _handler = new LoginWorkshopHandler(
            _authenticator,
            _tokenGenerator,
            _refreshTokenService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new LoginWorkshopCommand("test@workshop.com", "Password123");
        var identityId = Guid.NewGuid();
        var domainEntityId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var expectedRefreshToken = "test-refresh-token";

        var authenticatedIdentity = new AuthenticatedIdentity(
            identityId, domainEntityId, command.Email, EIdentityType.Workshop);

        var tokenDto = new TokenDTO("generated-token-string", DateTime.UtcNow.AddHours(1), jti);

        _authenticator.AuthenticateAsync(command.Email, command.Password, EIdentityType.Workshop)
            .Returns(authenticatedIdentity);

        _tokenGenerator.GenerateToken(authenticatedIdentity)
            .Returns(tokenDto);

        _refreshTokenService.GenerateRefreshTokenAsync(identityId, jti, Arg.Any<CancellationToken>())
            .Returns(expectedRefreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be(tokenDto.AccessToken);
        result.Value.ExpiresAtUtc.Should().Be(tokenDto.ExpiresAt);
        result.Value.WorkshopId.Should().Be(domainEntityId);
        result.Value.RefreshToken.Should().Be(expectedRefreshToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAuthenticationFails()
    {
        // Arrange
        var command = new LoginWorkshopCommand("wrong@workshop.com", "WrongPassword");
        var authError = Error.Validation(EField.General, "Invalid email or password.");

        _authenticator.AuthenticateAsync(command.Email, command.Password, EIdentityType.Workshop)
            .Returns(authError);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(authError);
        _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateToken(default!);
        await _refreshTokenService.DidNotReceiveWithAnyArgs().GenerateRefreshTokenAsync(default, default, default);
    }

    [Fact]
    public async Task Handle_Should_ReturnInternalError_WhenAuthenticatorReturnsNullValue()
    {
        // Arrange
        var command = new LoginWorkshopCommand("test@workshop.com", "Password123");

        _authenticator.AuthenticateAsync(command.Email, command.Password, EIdentityType.Workshop)
            .Returns(new Result<AuthenticatedIdentity, Error>((AuthenticatedIdentity)null!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.InternalServerError);
        _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateToken(default!);
        await _refreshTokenService.DidNotReceiveWithAnyArgs().GenerateRefreshTokenAsync(default, default, default);
    }
}
