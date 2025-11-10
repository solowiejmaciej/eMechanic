namespace eMechanic.Application.Tests.Users.Features.Register;

using eMechanic.Application.Abstractions.User;
using eMechanic.Application.Users.Features.Register;
using eMechanic.Common.Result;
using NSubstitute;

public class RegisterUserHandlerTests
{
    private readonly IUserCreatorService _userCreatorService;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _userCreatorService = Substitute.For<IUserCreatorService>();
        _handler = new RegisterUserHandler(_userCreatorService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var command = new RegisterUserCommand("Jan", "Kowalski", "jan@kowalski.pl", "Password123");
        var newUserId = Guid.NewGuid();

        _userCreatorService.CreateUserWithIdentityAsync(
            command.Email,
            command.Password,
            command.FirstName,
            command.LastName,
            Arg.Any<CancellationToken>())
        .Returns(newUserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.HasError());
        Assert.Equal(newUserId, result.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnErrorResult_WhenCreatorServiceFails()
    {
        // Arrange
        var command = new RegisterUserCommand("Jan", "Kowalski", "jan@kowalski.pl", "Password123");
        var error = new Error(EErrorCode.ValidationError, "Email already exists.");

        _userCreatorService.CreateUserWithIdentityAsync(
            command.Email,
            command.Password,
            command.FirstName,
            command.LastName,
            Arg.Any<CancellationToken>())
        .Returns(error);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.HasError());
        Assert.Equal(error.Code, result.Error!.Code);
    }
}
