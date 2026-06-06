namespace eMechanic.Domain.Workshop.Reviews.DomainEvents;

using Common.DDD;

public sealed record WorkshopReviewUpdatedDomainEvent(Review Review) : IDomainEvent;

