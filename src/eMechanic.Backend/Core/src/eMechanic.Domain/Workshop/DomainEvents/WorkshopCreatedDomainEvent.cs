namespace eMechanic.Domain.Workshop.DomainEvents;

using eMechanic.Common.DDD;
using Events.Events;
using Events.Events.Workshop;

public record WorkshopCreatedDomainEvent(Workshop Workshop) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent() => new WorkshopCreatedEvent(Workshop.Id, Workshop.Email, Workshop.ContactEmail, Workshop.Name, Workshop.DisplayName, Workshop.PhoneNumber, Workshop.Address, Workshop.City, Workshop.PostalCode, Workshop.Country);
}
