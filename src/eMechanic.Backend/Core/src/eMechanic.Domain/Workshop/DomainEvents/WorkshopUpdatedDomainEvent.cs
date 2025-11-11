namespace eMechanic.Domain.Workshop.DomainEvents;

using Common.DDD;
using Events.Events;
using Events.Events.Workshop;

public record WorkshopUpdatedDomainEvent(Workshop Workshop) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent() => new WorkshopUpdatedEvent(Workshop.Id, Workshop.Email, Workshop.ContactEmail, Workshop.Name, Workshop.DisplayName, Workshop.PhoneNumber, Workshop.Address, Workshop.City, Workshop.PostalCode, Workshop.Country);
}
