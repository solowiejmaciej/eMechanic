namespace eMechanic.Application.Tests.Users.Register;

using eMechanic.Application.Users.Register.UserCreated;
using eMechanic.Domain.Users;
using eMechanic.Domain.Users.DomainEvents;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class UserCreatedDomainEventHandlerTests
{
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;
    private readonly UserCreatedDomainEventHandler _handler;

    public UserCreatedDomainEventHandlerTests()
    {
        _logger = Substitute.For<ILogger<UserCreatedDomainEventHandler>>();
        _handler = new UserCreatedDomainEventHandler(_logger);
    }

    [Fact]
    public async Task Handle_Should_LogInformation_WhenEventIsReceived()
    {
        // Arrange
        var fakeUser = User.Create("test@test.pl", "Jan", "Kowalski", Guid.NewGuid());
        var domainEvent = new UserCreatedDomainEvent(fakeUser);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());

    }
}
