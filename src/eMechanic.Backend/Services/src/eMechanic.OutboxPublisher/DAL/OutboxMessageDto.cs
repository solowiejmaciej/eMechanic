namespace eMechanic.OutboxPublisher.DAL;

public record OutboxMessageDto(Guid Id, string Payload, string EventType);
