namespace eMechanic.Domain.Tests.UserRepairPreferences;

using System;
using Domain.UserRepairPreferences;
using Domain.UserRepairPreferences.Enums;
using eMechanic.Domain.Tests.Builders;
using FluentAssertions;

public class UserRepairPreferencesTests
{
    private readonly Guid _validUserId = Guid.NewGuid();

    [Theory]
    [InlineData(EPartsPreference.Economy, ETimelinePreference.Standard)]
    [InlineData(EPartsPreference.Premium, ETimelinePreference.Urgent)]
    public void Create_Should_ReturnUserRepairPreferences_WhenCreatedWithValidData(
        EPartsPreference partsPref, ETimelinePreference timelinePref)
    {
        // Act
        var preferences = new UserRepairPreferencesBuilder()
            .WithUserId(_validUserId)
            .WithPartsPreference(partsPref)
            .WithTimelinePreference(timelinePref)
            .Build();

        // Assert
        preferences.Should().NotBeNull();
        preferences.UserId.Should().Be(_validUserId);
        preferences.PartsPreference.Should().Be(partsPref);
        preferences.TimelinePreference.Should().Be(timelinePref);
    }

    [Fact]
    public void Create_Should_ThrowArgumentException_WhenUserIdIsEmpty()
    {
        // Act
        Action act = () => new UserRepairPreferencesBuilder().WithUserId(Guid.Empty).Build();

        // Assert
        act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("userId");
    }

    [Fact]
    public void UpdatePreferences_Should_UpdatePreferencesAndRaiseEvent()
    {
        // Arrange
        var preferences = new UserRepairPreferencesBuilder().Build();
        preferences.ClearDomainEvents();

        // Act
        preferences.UpdatePartsPreference(EPartsPreference.Premium);
        preferences.UpdateTimelinePreference(ETimelinePreference.Urgent);

        // Assert
        preferences.PartsPreference.Should().Be(EPartsPreference.Premium);
        preferences.TimelinePreference.Should().Be(ETimelinePreference.Urgent);
    }

    [Fact]
    public void UpdatePreferences_Should_NotRaiseEvent_WhenPreferencesAreTheSame()
    {
        // Arrange
        var preferences = new UserRepairPreferencesBuilder().Build();
        preferences.ClearDomainEvents();

        // Act
        preferences.UpdatePartsPreference(EPartsPreference.Economy);
        preferences.UpdateTimelinePreference(ETimelinePreference.Standard);

        // Assert
        preferences.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void SetPartsPreference_Should_UpdatePreferenceAndRaiseEvent()
    {
        // Arrange
        var preferences = new UserRepairPreferencesBuilder().Build();
        preferences.ClearDomainEvents();

        // Act
        preferences.UpdatePartsPreference(EPartsPreference.Premium);

        // Assert
        preferences.PartsPreference.Should().Be(EPartsPreference.Premium);
    }

    [Fact]
    public void SetPartsPreference_Should_NotRaiseEvent_WhenPreferenceIsTheSame()
    {
        // Arrange
        var preferences = new UserRepairPreferencesBuilder().Build();
        preferences.ClearDomainEvents();

        // Act
        preferences.UpdatePartsPreference(EPartsPreference.Economy);

        // Assert
        preferences.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void SetTimelinePreference_Should_UpdatePreferenceAndRaiseEvent()
    {
        // Arrange
        var preferences = new UserRepairPreferencesBuilder().Build();
        preferences.ClearDomainEvents();

        // Act
        preferences.UpdateTimelinePreference(ETimelinePreference.Urgent);

        // Assert
        preferences.TimelinePreference.Should().Be(ETimelinePreference.Urgent);
    }

    [Fact]
    public void SetTimelinePreference_Should_NotRaiseEvent_WhenPreferenceIsTheSame()
    {
        // Arrange
        var preferences = new UserRepairPreferencesBuilder().Build();
        preferences.ClearDomainEvents();

        // Act
        preferences.UpdateTimelinePreference(ETimelinePreference.Standard);

        // Assert
        preferences.GetDomainEvents().Should().BeEmpty();
    }
}
