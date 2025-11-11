namespace eMechanic.Application.Tests.Users.Features.Create;

using eMechanic.Application.Users.Features.Create;
using eMechanic.Application.Users.Services;
using eMechanic.Common.Result;
using NSubstitute;

public class CreateUserHandlerTests
{
    private readonly IUserService _userService;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _handler = new CreateUserHandler(_userService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var command = new CreateUserCommand("Jan", "Kowalski", "jan@kowalski.pl", "Password123");
        var newUserId = Guid.NewGuid();

        _userService.CreateUserWithIdentityAsync(
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
        var command = new CreateUserCommand("Jan", "Kowalski", "jan@kowalski.pl", "Password123");
        var error = new Error(EErrorCode.ValidationError, "Email already exists.");

        _userService.CreateUserWithIdentityAsync(
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
