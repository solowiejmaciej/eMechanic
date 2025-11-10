namespace eMechanic.Application.Tests.UserPreferences.Features.Update;

using Abstractions.UserRepairPreferences;
using Domain.UserRepairPreferences;
using Domain.UserRepairPreferences.Enums;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Common.Result;
using FluentAssertions;
using NSubstitute;
using UserRepairPreferences.Features.Update;

public class UpdateUserRepairPreferencesHandlerTests
{
    private readonly IUserRepairPreferencesRepository _preferencesRepository;
    private readonly IUserContext _userContext;
    private readonly UpdateUserRepairPreferencesHandler _handler;
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly UserRepairPreferences _existingPreferences;

    public UpdateUserRepairPreferencesHandlerTests()
    {
        _preferencesRepository = Substitute.For<IUserRepairPreferencesRepository>();
        _userContext = Substitute.For<IUserContext>();

        // Tworzymy instancję agregatu do testów
        _existingPreferences = UserRepairPreferences.Create(
            _currentUserId,
            EPartsPreference.Economy,
            ETimelinePreference.Standard);

        _handler = new UpdateUserRepairPreferencesHandler(_preferencesRepository, _userContext);

        _userContext.GetUserId().Returns(_currentUserId);
        _preferencesRepository.GetByUserIdAsync(_currentUserId, Arg.Any<CancellationToken>())
            .Returns(_existingPreferences);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_AndUpdateDomainObject_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateUserRepairPreferencesCommand(
            EPartsPreference.Premium,
            ETimelinePreference.Urgent);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();

        _existingPreferences.PartsPreference.Should().Be(EPartsPreference.Premium);
        _existingPreferences.TimelinePreference.Should().Be(ETimelinePreference.Urgent);

        await _preferencesRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenPreferencesNotFound()
    {
        // Arrange
        _preferencesRepository.GetByUserIdAsync(_currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<UserRepairPreferences?>(null));

        var command = new UpdateUserRepairPreferencesCommand(EPartsPreference.Premium, ETimelinePreference.Urgent);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);

        await _preferencesRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotSaveChanges_WhenPreferencesAreTheSame()
    {
        // Arrange
        var command = new UpdateUserRepairPreferencesCommand(
            EPartsPreference.Economy,
            ETimelinePreference.Standard);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();

        _existingPreferences.PartsPreference.Should().Be(EPartsPreference.Economy);
        _existingPreferences.TimelinePreference.Should().Be(ETimelinePreference.Standard);
        await _preferencesRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
