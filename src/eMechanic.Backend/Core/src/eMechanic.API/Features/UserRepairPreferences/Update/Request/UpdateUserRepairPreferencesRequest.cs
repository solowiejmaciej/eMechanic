namespace eMechanic.API.Features.UserRepairPreferences.Update.Request;

using Application.UserRepairPreferences.Features.Update;
using eMechanic.Domain.UserRepairPreferences.Enums;

public sealed record UpdateUserRepairPreferencesRequest(
    EPartsPreference PartsPreference,
    ETimelinePreference TimelinePreference)
{
    public UpdateUserRepairPreferencesCommand ToCommand() => new(PartsPreference, TimelinePreference);
}
