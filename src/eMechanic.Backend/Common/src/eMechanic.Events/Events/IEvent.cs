namespace eMechanic.Events.Events;

public interface IEvent
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
}
