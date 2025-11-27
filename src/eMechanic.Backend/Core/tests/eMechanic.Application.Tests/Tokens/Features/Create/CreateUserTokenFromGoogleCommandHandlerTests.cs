namespace eMechanic.Application.Tests.Tokens.Features.Create;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Application.Token.Features.Create.User.External;
using eMechanic.Common.Result;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class CreateUserTokenFromGoogleCommandHandlerTests
{
    private readonly IGoogleAuthService _googleAuthServiceMock;
    private readonly ITokenGenerator _tokenGeneratorMock;
    private readonly IRefreshTokenService _refreshTokenServiceMock;
    private readonly CreateUserTokenFromGoogleCommandHandler _handler;

    public CreateUserTokenFromGoogleCommandHandlerTests()
    {
        _googleAuthServiceMock = Substitute.For<IGoogleAuthService>();
        _tokenGeneratorMock = Substitute.For<ITokenGenerator>();
        _refreshTokenServiceMock = Substitute.For<IRefreshTokenService>();

        _handler = new CreateUserTokenFromGoogleCommandHandler(
            _googleAuthServiceMock,
            _tokenGeneratorMock,
            _refreshTokenServiceMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenGoogleLoginFails()
    {
        // Arrange
        var command = new CreateUserTokenFromGoogleCommand("invalid_token");
        var error = new Error(EErrorCode.ValidationError, "Invalid token");

        _googleAuthServiceMock
            .LoginAsync(command.IdToken, Arg.Any<CancellationToken>())
            .Returns(error);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldReturnTokenResponse_WhenLoginSucceeds()
    {
        // Arrange
        var command = new CreateUserTokenFromGoogleCommand("valid_token");
        var identityId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var authenticatedIdentity = new AuthenticatedIdentity(identityId, userId, "test@test.com", EIdentityType.User);

        var accessToken = "access_jwt_token";
        var refreshToken = "refresh_token_xyz";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        _googleAuthServiceMock
            .LoginAsync(command.IdToken, Arg.Any<CancellationToken>())
            .Returns(authenticatedIdentity);

        _tokenGeneratorMock
            .GenerateToken(authenticatedIdentity)
            .Returns(new TokenDTO(accessToken, expiresAt, Guid.NewGuid()));

        _refreshTokenServiceMock
            .GenerateRefreshTokenAsync(identityId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().Be(accessToken);
        result.Value.RefreshToken.Should().Be(refreshToken);
        result.Value.UserId.Should().Be(userId);
    }
}
