using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Application.Workshop.Login;
using eMechanic.Common.Result;
using eMechanic.Common.Result.Fields;
using NSubstitute;

namespace eMechanic.Application.Tests.Workshop.Login;

public class LoginWorkshopHandlerTests
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly LoginWorkshopHandler _handler;

    public LoginWorkshopHandlerTests()
    {
        _authenticator = Substitute.For<IAuthenticator>();
        _tokenGenerator = Substitute.For<ITokenGenerator>();
        _handler = new LoginWorkshopHandler(_authenticator, _tokenGenerator);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new LoginWorkshopCommand("test@workshop.com", "Password123");
        var identityId = Guid.NewGuid();
        var domainEntityId = Guid.NewGuid();

        var authenticatedIdentity = new AuthenticatedIdentity(
            identityId, domainEntityId, command.Email, EIdentityType.Workshop);

        var tokenDto = new TokenDTO("generated-token-string", DateTime.UtcNow.AddHours(1));

        _authenticator.AuthenticateAsync(command.Email, command.Password, EIdentityType.Workshop)
            .Returns(authenticatedIdentity);

        _tokenGenerator.GenerateToken(authenticatedIdentity)
            .Returns(tokenDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.HasError());
        Assert.NotNull(result.Value);
        Assert.Equal(tokenDto.AccessToken, result.Value.Token);
        Assert.Equal(tokenDto.ExpiresAt, result.Value.ExpiresAtUtc);
        Assert.Equal(domainEntityId, result.Value.WorkshopId);
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
        Assert.True(result.HasError());
        Assert.Equal(authError, result.Error);
        _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateToken(default!);
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
        Assert.True(result.HasError());
        Assert.Equal(EErrorCode.InternalServerError, result.Error!.Code);
        _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateToken(default!);
    }
}
