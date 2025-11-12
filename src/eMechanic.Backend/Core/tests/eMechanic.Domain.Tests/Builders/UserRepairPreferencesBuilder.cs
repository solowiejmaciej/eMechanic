namespace eMechanic.Domain.Tests.Builders;

using eMechanic.Domain.UserRepairPreferences;
using eMechanic.Domain.UserRepairPreferences.Enums;

public class UserRepairPreferencesBuilder
{
    private Guid _userId = Guid.NewGuid();
    private EPartsPreference _partsPreference = EPartsPreference.Economy;
    private ETimelinePreference _timelinePreference = ETimelinePreference.Standard;

    public UserRepairPreferencesBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public UserRepairPreferencesBuilder WithPartsPreference(EPartsPreference partsPreference)
    {
        _partsPreference = partsPreference;
        return this;
    }

    public UserRepairPreferencesBuilder WithTimelinePreference(ETimelinePreference timelinePreference)
    {
        _timelinePreference = timelinePreference;
        return this;
    }

    public UserRepairPreferences Build()
    {
        return UserRepairPreferences.Create(_userId, _partsPreference, _timelinePreference);
    }
}
