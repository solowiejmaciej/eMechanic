namespace eMechanic.Application.Tests.WorkshopReviews.Features.Upsert;

using Application.Abstractions.Identity.Contexts;
using Application.Workshop.Reviews.Features.Upsert;
using Application.Workshop.Reviews.Repositories;
using Application.Workshop.Workshop.Repositories;
using Common.Result;
using Domain.Workshop;
using Domain.Workshop.Reviews;
using FluentAssertions;
using NSubstitute;

public class UpsertWorkshopReviewCommandHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IWorkshopReviewRepository _reviewRepository = Substitute.For<IWorkshopReviewRepository>();
    private readonly IWorkshopRepository _workshopRepository = Substitute.For<IWorkshopRepository>();
    private readonly UpsertWorkshopReviewCommandHandler _handler;

    public UpsertWorkshopReviewCommandHandlerTests()
    {
        _handler = new UpsertWorkshopReviewCommandHandler(_userContext, _reviewRepository, _workshopRepository);
    }

    [Fact]
    public async Task Handle_Should_CreateNewReview_WhenReviewDoesNotExist()
    {
        // Arrange
        var command = new UpsertWorkshopReviewCommand(Guid.NewGuid(), 5, "Excellent");
        var userId = Guid.NewGuid();
        var workshop = new Domain.Tests.Builders.WorkshopBuilder().Build();

        _userContext.GetUserId().Returns(userId);
        _workshopRepository.GetByIdAsync(command.WorkshopId, Arg.Any<CancellationToken>()).Returns(workshop);
        _reviewRepository.GetForWorkshopByUserAsTrackingAsync(command.WorkshopId, userId, Arg.Any<CancellationToken>())
            .Returns((Review?)null);
        _reviewRepository.AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>()).Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        await _reviewRepository.Received(1).AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
        await _reviewRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenWorkshopDoesNotExist()
    {
        // Arrange
        var command = new UpsertWorkshopReviewCommand(Guid.NewGuid(), 4, "Good");
        _workshopRepository.GetByIdAsync(command.WorkshopId, Arg.Any<CancellationToken>()).Returns((Workshop?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        await _reviewRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

