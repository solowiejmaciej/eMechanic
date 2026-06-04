namespace eMechanic.Domain.UserRepairPreferences;

using Common.DDD;
using DomainEvents;
using Enums;
using Shared.References.User;

public class UserRepairPreferences : AggregateRoot, IUserReferenced
{
    public Guid UserId { get; private set; }
    public EPartsPreference PartsPreference { get; private set; }
    public ETimelinePreference TimelinePreference { get; private set; }

    private UserRepairPreferences() { }

    private UserRepairPreferences(
        Guid userId,
        EPartsPreference partsPreference,
        ETimelinePreference timelinePreference)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        }

        UserId = userId;
        PartsPreference = partsPreference;
        TimelinePreference = timelinePreference;
    }

    public static UserRepairPreferences Create(
        Guid userId,
        EPartsPreference partsPreference,
        ETimelinePreference timelinePreference)
    {
        return new UserRepairPreferences(userId, partsPreference, timelinePreference);
    }

    public void UpdatePartsPreference(EPartsPreference newPreference)
    {
        if (!Enum.IsDefined(newPreference))
        {
            throw new ArgumentException("Invalid PartsPreference value", nameof(newPreference));
        }

        if (PartsPreference == newPreference)
        {
            return;
        }

        var oldValue = PartsPreference;
        PartsPreference = newPreference;
        RaiseDomainEvent(new UserRepairPartsPreferenceUpdatedDomainEvent(UserId, oldValue, newPreference));
    }

    public void UpdateTimelinePreference(ETimelinePreference newPreference)
    {
        if (!Enum.IsDefined(newPreference))
        {
            throw new ArgumentException("Invalid TimelinePreference value", nameof(newPreference));
        }

        if (TimelinePreference == newPreference)
        {
            return;
        }

        var oldValue = TimelinePreference;
        TimelinePreference = newPreference;
        RaiseDomainEvent(new UserRepairTimelinePreferenceUpdatedDomainEvent(UserId, oldValue, newPreference));
    }
}
