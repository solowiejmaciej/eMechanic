namespace eMechanic.Events.Events;

public class EventBase : IEvent
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
