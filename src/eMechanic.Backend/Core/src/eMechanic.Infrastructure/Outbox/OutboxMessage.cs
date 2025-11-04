namespace eMechanic.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string EventType { get; private set; }
    public string Payload { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private OutboxMessage() { }

    public OutboxMessage(string eventType, string payload)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        Payload = payload;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessed() => ProcessedAt = DateTime.UtcNow;
}
