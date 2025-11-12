namespace eMechanic.Application.Tests.Users.DomainEventHandlers;

using Application.Tests.Builders;
using Domain.UserRepairPreferences;
using eMechanic.Domain.User;
using eMechanic.Domain.User.DomainEvents;
using eMechanic.Domain.UserRepairPreferences.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using eMechanic.Application.Users.DomainEventsHandlers;
using eMechanic.Domain.Tests.Builders;
using UserRepairPreferences.Repositories;

public class CreateUserRepairPreferencesUserCreatedDomainEventHandlerTests
{
    private readonly IUserRepairPreferencesRepository _preferencesRepository;
    private readonly ILogger<CreateUserRepairPreferencesUserCreatedDomainEventHandler> _logger;
    private readonly CreateUserRepairPreferencesUserCreatedDomainEventHandler _handler;

    public CreateUserRepairPreferencesUserCreatedDomainEventHandlerTests()
    {
        _preferencesRepository = Substitute.For<IUserRepairPreferencesRepository>();
        _logger = Substitute.For<ILogger<CreateUserRepairPreferencesUserCreatedDomainEventHandler>>();
        _handler = new CreateUserRepairPreferencesUserCreatedDomainEventHandler(_preferencesRepository, _logger);
    }

    [Fact]
    public async Task Handle_Should_CreateDefaultPreferences_WhenUserIsCreated()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var domainEvent = new UserCreatedDomainEvent(user);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        await _preferencesRepository.Received(1).AddAsync(
            Arg.Is<UserRepairPreferences>(p =>
                p.UserId == user.Id &&
                p.PartsPreference == EPartsPreference.Balanced &&
                p.TimelinePreference == ETimelinePreference.Standard),
            Arg.Any<CancellationToken>());
    }
}
