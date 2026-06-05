namespace eMechanic.Domain.Tests.Workshop;

using Domain.Workshop.Reviews;
using Domain.Workshop.Reviews.DomainEvents;
using FluentAssertions;

public class WorkshopReviewTests
{
    [Fact]
    public void Create_Should_ReturnReviewAndRaiseCreatedEvent_WhenPayloadIsValid()
    {
        // Arrange
        var workshopId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var result = Review.Create(workshopId, userId, 5, "Great service");

        // Assert
        result.HasError().Should().BeFalse();
        result.Value!.WorkshopId.Should().Be(workshopId);
        result.Value.UserId.Should().Be(userId);
        result.Value.Rating.Value.Should().Be(5);
        result.Value.Comment!.Value.Should().Be("Great service");
        result.Value.GetDomainEvents().Should().ContainSingle(x => x is WorkshopReviewCreatedDomainEvent);
    }

    [Fact]
    public void Create_Should_ReturnValidationError_WhenRatingIsOutOfRange()
    {
        // Act
        var result = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 0, "bad");

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(Common.Result.EErrorCode.ValidationError);
    }

    [Fact]
    public void Update_Should_UpdateStateAndRaiseUpdatedEvent_WhenPayloadIsValid()
    {
        // Arrange
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 4, "Good").Value!;
        review.ClearDomainEvents();

        // Act
        var result = review.Update(5, "Perfect");

        // Assert
        result.HasError().Should().BeFalse();
        review.Rating.Value.Should().Be(5);
        review.Comment!.Value.Should().Be("Perfect");
        review.GetDomainEvents().Should().ContainSingle(x => x is WorkshopReviewUpdatedDomainEvent);
    }

    [Fact]
    public void Delete_Should_RaiseDeletedEvent()
    {
        // Arrange
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 3, null).Value!;
        review.ClearDomainEvents();

        // Act
        var result = review.Delete();

        // Assert
        result.HasError().Should().BeFalse();
        review.GetDomainEvents().Should().ContainSingle(x => x is WorkshopReviewDeletedDomainEvent);
    }
}

