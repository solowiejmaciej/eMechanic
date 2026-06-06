namespace eMechanic.Application.Workshop.Reviews.Features.Get.Stats;

using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class GetWorkshopReviewStatsQueryHandler : IResultQueryHandler<GetWorkshopReviewStatsQuery, WorkshopReviewStatsResponse>
{
    private readonly IWorkshopReviewRepository _reviewRepository;

    public GetWorkshopReviewStatsQueryHandler(IWorkshopReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<WorkshopReviewStatsResponse, Error>> Handle(
        GetWorkshopReviewStatsQuery request,
        CancellationToken cancellationToken)
    {
        var projection = await _reviewRepository.GetStatsForWorkshopAsync(request.WorkshopId, cancellationToken);

        return new WorkshopReviewStatsResponse(
            projection.WorkshopId,
            decimal.Round(projection.AverageRating, 2, MidpointRounding.AwayFromZero),
            projection.TotalReviews);
    }
}

