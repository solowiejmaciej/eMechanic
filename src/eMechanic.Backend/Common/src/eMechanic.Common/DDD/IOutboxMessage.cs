namespace eMechanic.Common.DDD;

using Events.Events;

public interface IOutboxMessage
{
    IEvent MapToEvent();
}
