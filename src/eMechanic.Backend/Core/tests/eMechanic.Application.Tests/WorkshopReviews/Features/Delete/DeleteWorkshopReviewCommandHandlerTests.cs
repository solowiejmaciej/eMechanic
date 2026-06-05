namespace eMechanic.Application.Tests.WorkshopReviews.Features.Delete;

using Application.Abstractions.Identity.Contexts;
using Application.Workshop.Reviews.Features.Delete;
using Application.Workshop.Reviews.Repositories;
using Common.Result;
using Domain.Workshop.Reviews;
using FluentAssertions;
using NSubstitute;

public class DeleteWorkshopReviewCommandHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IWorkshopReviewRepository _reviewRepository = Substitute.For<IWorkshopReviewRepository>();
    private readonly DeleteWorkshopReviewCommandHandler _handler;

    public DeleteWorkshopReviewCommandHandlerTests()
    {
        _handler = new DeleteWorkshopReviewCommandHandler(_userContext, _reviewRepository);
    }

    [Fact]
    public async Task Handle_Should_DeleteReview_WhenReviewExists()
    {
        // Arrange
        var workshopId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteWorkshopReviewCommand(workshopId);
        var review = Review.Create(workshopId, userId, 3, "ok").Value!;

        _userContext.GetUserId().Returns(userId);
        _reviewRepository.GetForWorkshopByUserAsTrackingAsync(workshopId, userId, Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        await _reviewRepository.Received(1).DeleteAsync(review, Arg.Any<CancellationToken>());
        await _reviewRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenReviewDoesNotExist()
    {
        // Arrange
        var workshopId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.GetUserId().Returns(userId);
        _reviewRepository.GetForWorkshopByUserAsTrackingAsync(workshopId, userId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);

        // Act
        var result = await _handler.Handle(new DeleteWorkshopReviewCommand(workshopId), CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }
}

