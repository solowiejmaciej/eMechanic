namespace eMechanic.Application.Tests.Tokens.Features.Refresh;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Application.Token.Features.Refresh;
using eMechanic.Common.Result;
using FluentAssertions;
using NSubstitute;

public class RefreshTokenCommandHandlerTests
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenService = Substitute.For<IRefreshTokenService>();
        _handler = new RefreshTokenCommandHandler(_refreshTokenService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResponse_WhenTokenRotationSucceeds()
    {
        // Arrange
        var command = new RefreshTokenCommand("oldRefreshToken", "oldAccessToken");

        var serviceResponseDto = new RefreshTokenDTO(
            "new-access-token",
            DateTime.UtcNow.AddHours(1),
            Guid.NewGuid(),
            "new-refresh-token",
            EIdentityType.User
        );

        _refreshTokenService.ValidateAndRotateRefreshTokenAsync(command.AccessToken, command.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<RefreshTokenDTO, Error>>(serviceResponseDto));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be(serviceResponseDto.NewAccessToken);
        result.Value.RefreshToken.Should().Be(serviceResponseDto.NewRefreshToken);
        result.Value.ExpiresAtUtc.Should().Be(serviceResponseDto.NewExpiresAt);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenTokenRotationFails()
    {
        // Arrange
        var command = new RefreshTokenCommand("invalidRefreshToken", "expiredAccessToken");
        var error = new Error(EErrorCode.UnauthorizedError, "Invalid token.");

        _refreshTokenService.ValidateAndRotateRefreshTokenAsync(command.AccessToken, command.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<RefreshTokenDTO, Error>>(error));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
