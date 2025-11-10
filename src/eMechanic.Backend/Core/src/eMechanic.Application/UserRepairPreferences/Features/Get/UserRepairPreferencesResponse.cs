namespace eMechanic.Application.UserRepairPreferences.Features.Get;

using eMechanic.Domain.UserRepairPreferences.Enums;

public sealed record UserRepairPreferencesResponse(
    Guid Id,
    Guid UserId,
    EPartsPreference PartsPreference,
    ETimelinePreference TimelinePreference);
