namespace eMechanic.Application.Tests.WorkshopReviews.Features.Get.Stats;

using Application.Workshop.Reviews.Features.Get.Stats;
using Application.Workshop.Reviews.Repositories;
using FluentAssertions;
using NSubstitute;

public class GetWorkshopReviewStatsQueryHandlerTests
{
    private readonly IWorkshopReviewRepository _reviewRepository = Substitute.For<IWorkshopReviewRepository>();
    private readonly GetWorkshopReviewStatsQueryHandler _handler;

    public GetWorkshopReviewStatsQueryHandlerTests()
    {
        _handler = new GetWorkshopReviewStatsQueryHandler(_reviewRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnStatsProjectionData()
    {
        // Arrange
        var workshopId = Guid.NewGuid();
        _reviewRepository.GetStatsForWorkshopAsync(workshopId, Arg.Any<CancellationToken>())
            .Returns(new WorkshopReviewStatsProjection(workshopId, 4.666m, 3));

        // Act
        var result = await _handler.Handle(new GetWorkshopReviewStatsQuery(workshopId), CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value!.WorkshopId.Should().Be(workshopId);
        result.Value.AverageRating.Should().Be(4.67m);
        result.Value.TotalReviews.Should().Be(3);
    }
}

