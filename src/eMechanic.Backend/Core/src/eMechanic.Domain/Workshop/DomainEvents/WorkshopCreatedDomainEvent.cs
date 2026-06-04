namespace eMechanic.Domain.Workshop.DomainEvents;

using eMechanic.Common.DDD;
using Events.Events;
using Events.Events.Workshop;

public record WorkshopCreatedDomainEvent(Workshop Workshop) : IDomainEvent, IOutboxMessage
{
    public IEvent MapToEvent() => new WorkshopCreatedEvent(
        Workshop.Id,
        Workshop.Email.Value,
        Workshop.ContactEmail.Value,
        Workshop.Name,
        Workshop.DisplayName,
        Workshop.PhoneNumber.Value,
        Workshop.Address,
        Workshop.City,
        Workshop.PostalCode,
        Workshop.Country);
}
