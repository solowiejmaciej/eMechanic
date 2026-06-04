namespace eMechanic.Domain.Workshop.Reviews.DomainEvents;

using Common.DDD;

public sealed record WorkshopReviewCreatedDomainEvent(Review Review) : IDomainEvent;

