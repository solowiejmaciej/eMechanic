namespace eMechanic.Domain.UserRepairPreferences.DomainEvents;

using Common.DDD;
using Enums;

public sealed record UserRepairPartsPreferenceUpdatedDomainEvent(
    Guid UserId,
    EPartsPreference OldValue,
    EPartsPreference NewValue) : IDomainEvent;

