namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.UserRepairPreferences.Features.Update;
using eMechanic.Domain.UserRepairPreferences.Enums;

public class UpdateUserRepairPreferencesCommandBuilder
{
    private EPartsPreference _partsPreference = EPartsPreference.Economy;
    private ETimelinePreference _timelinePreference = ETimelinePreference.Standard;

    public UpdateUserRepairPreferencesCommandBuilder WithPartsPreference(EPartsPreference partsPreference)
    {
        _partsPreference = partsPreference;
        return this;
    }

    public UpdateUserRepairPreferencesCommandBuilder WithTimelinePreference(ETimelinePreference timelinePreference)
    {
        _timelinePreference = timelinePreference;
        return this;
    }

    public UpdateUserRepairPreferencesCommand Build()
    {
        return new UpdateUserRepairPreferencesCommand(_partsPreference, _timelinePreference);
    }
}
