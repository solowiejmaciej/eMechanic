namespace eMechanic.Domain.Workshop.Reviews.DomainEvents;

using Common.DDD;

public sealed record WorkshopReviewDeletedDomainEvent(Review Review) : IDomainEvent;

