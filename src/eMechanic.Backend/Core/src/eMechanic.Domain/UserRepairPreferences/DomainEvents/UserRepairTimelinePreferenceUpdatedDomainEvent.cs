namespace eMechanic.Domain.UserRepairPreferences.DomainEvents;

using Common.DDD;
using Enums;

public sealed record UserRepairTimelinePreferenceUpdatedDomainEvent(
    Guid UserId,
    ETimelinePreference OldValue,
    ETimelinePreference NewValue) : IDomainEvent;

