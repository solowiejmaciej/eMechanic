namespace eMechanic.Events.Factories;

using Events;

public record EventFactoryResult(IEvent? Event, Type? Type);
