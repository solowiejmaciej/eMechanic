using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Application.Users.Login;
using eMechanic.Common.Result;
using eMechanic.Common.Result.Fields;
using NSubstitute;

namespace eMechanic.Application.Tests.Users.Login;

public class LoginUserHandlerTests
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _authenticator = Substitute.For<IAuthenticator>();
        _tokenGenerator = Substitute.For<ITokenGenerator>();
        _handler = new LoginUserHandler(_authenticator, _tokenGenerator);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new LoginUserCommand("test@user.com", "Password123");
        var identityId = Guid.NewGuid();

        var authenticatedIdentity = new AuthenticatedIdentity(
            identityId, command.Email, EIdentityType.User);

        var tokenDto = new TokenDTO("generated-token-string", DateTime.UtcNow.AddHours(1));

        _authenticator.AuthenticateAsync(command.Email, command.Password, EIdentityType.User)
            .Returns(authenticatedIdentity);

        _tokenGenerator.GenerateToken(authenticatedIdentity)
            .Returns(tokenDto);

        // Act
        // Zakładamy, że handler zwraca LoginUserResponse
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.HasError());
        Assert.NotNull(result.Value);
        Assert.Equal(tokenDto.AccessToken, result.Value.Token);
        Assert.Equal(tokenDto.ExpiresAt, result.Value.ExpiresAtUtc);
        Assert.Equal(identityId, result.Value.UserId);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAuthenticationFails()
    {
        // Arrange
        var command = new LoginUserCommand("wrong@user.com", "WrongPassword");
        var authError = Error.Validation(EField.General, "Invalid email or password.");

        _authenticator.AuthenticateAsync(command.Email, command.Password, EIdentityType.User)
            .Returns(authError);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.HasError());
        Assert.Equal(authError, result.Error);
        _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateToken(default!);
    }

    [Fact]
    public async Task Handle_Should_ReturnInternalError_WhenAuthenticatorReturnsNullValue()
    {
        // Arrange
        var command = new LoginUserCommand("test@user.com", "Password123");

        _authenticator.AuthenticateAsync(command.Email, command.Password, EIdentityType.User)
            .Returns(new Result<AuthenticatedIdentity, Error>((AuthenticatedIdentity)null!));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.HasError());
        Assert.Equal(EErrorCode.InternalServerError, result.Error!.Code);
        _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateToken(default!);
    }
}
