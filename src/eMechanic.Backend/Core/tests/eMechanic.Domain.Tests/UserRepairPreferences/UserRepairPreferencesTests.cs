namespace eMechanic.Domain.Tests.UserRepairPreferences;

using eMechanic.Domain.UserRepairPreferences.Enums;
using FluentAssertions;
using Domain.UserRepairPreferences;

public class UserRepairPreferencesTests
{
    private readonly Guid _validUserId = Guid.NewGuid();

    [Fact]
    public void Create_Should_Succeed_WhenDataIsValid()
    {
        // Arrange
        var partsPref = EPartsPreference.Balanced;
        var timelinePref = ETimelinePreference.Standard;

        // Act
        var preferences = UserRepairPreferences.Create(_validUserId, partsPref, timelinePref);

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
        Action act = () => UserRepairPreferences.Create(
            Guid.Empty,
            EPartsPreference.Balanced,
            ETimelinePreference.Standard);

        // Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("userId");
    }

    [Fact]
    public void UpdatePartsPreference_Should_UpdateValue_WhenNewPreferenceIsValid()
    {
        // Arrange
        var preferences = UserRepairPreferences.Create(_validUserId, EPartsPreference.Economy, ETimelinePreference.Standard);
        var newPreference = EPartsPreference.Premium;

        // Act
        preferences.UpdatePartsPreference(newPreference);

        // Assert
        preferences.PartsPreference.Should().Be(newPreference);
    }

    [Fact]
    public void UpdatePartsPreference_Should_DoNothing_WhenPreferenceIsTheSame()
    {
        // Arrange
        var preferences = UserRepairPreferences.Create(_validUserId, EPartsPreference.Economy, ETimelinePreference.Standard);
        var samePreference = EPartsPreference.Economy;

        // Act
        preferences.UpdatePartsPreference(samePreference);

        // Assert
        preferences.PartsPreference.Should().Be(samePreference);
    }

    [Fact]
    public void UpdatePartsPreference_Should_ThrowArgumentException_WhenPreferenceIsInvalid()
    {
        // Arrange
        var preferences = UserRepairPreferences.Create(_validUserId, EPartsPreference.Economy, ETimelinePreference.Standard);
        var invalidPreference = (EPartsPreference)999;

        // Act
        Action act = () => preferences.UpdatePartsPreference(invalidPreference);

        // Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("newPreference");
    }

    [Fact]
    public void UpdateTimelinePreference_Should_UpdateValue_WhenNewPreferenceIsValid()
    {
        // Arrange
        var preferences = UserRepairPreferences.Create(_validUserId, EPartsPreference.Economy, ETimelinePreference.Standard);
        var newPreference = ETimelinePreference.Urgent;

        // Act
        preferences.UpdateTimelinePreference(newPreference);

        // Assert
        preferences.TimelinePreference.Should().Be(newPreference);
    }

    [Fact]
    public void UpdateTimelinePreference_Should_ThrowArgumentException_WhenPreferenceIsInvalid()
    {
        // Arrange
        var preferences = UserRepairPreferences.Create(_validUserId, EPartsPreference.Economy, ETimelinePreference.Standard);
        var invalidPreference = (ETimelinePreference)999;

        // Act
        Action act = () => preferences.UpdateTimelinePreference(invalidPreference);

        // Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("newPreference");
    }
}
