namespace eMechanic.Application.Tests.Users.Features.Update;

using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.Services;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Users.Features.Update;
using eMechanic.Common.Result;
using eMechanic.Common.Result.Fields;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class UpdateUserHandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IUserService _userService;
    private readonly UpdateUserHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();

    public UpdateUserHandlerTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _userService = Substitute.For<IUserService>();

        _handler = new UpdateUserHandler(_userContext, _userService);

        _userContext.GetUserId().Returns(_currentUserId);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserIsAuthenticatedAndServiceSucceeds()
    {
        // Arrange
        var command = new UpdateUserCommand("NewName", "NewLastName", "new@email.com");

        _userService.UpdateUserWithIdentityAsync(
            _currentUserId,
            command.Email,
            command.FirstName,
            command.LastName,
            Arg.Any<CancellationToken>())
        .Returns(Result.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();

        await _userService.Received(1).UpdateUserWithIdentityAsync(
            _currentUserId,
            command.Email,
            command.FirstName,
            command.LastName,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenServiceReturnsError()
    {
        // Arrange
        var command = new UpdateUserCommand("Jan", "Kowalski", "taken@email.com");
        var serviceError = Error.Validation(EField.Email, "Email already in use.");

        _userService.UpdateUserWithIdentityAsync(
            _currentUserId,
            command.Email,
            command.FirstName,
            command.LastName,
            Arg.Any<CancellationToken>())
        .Returns(serviceError);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(serviceError);
    }

    [Fact]
    public async Task Handle_Should_ReturnUnauthorizedError_WhenUserContextThrowsUnauthorized()
    {
        // Arrange
        var command = new UpdateUserCommand("Jan", "Kowalski", "new@email.com");

        _userContext.GetUserId().Throws<UnauthorizedAccessException>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
