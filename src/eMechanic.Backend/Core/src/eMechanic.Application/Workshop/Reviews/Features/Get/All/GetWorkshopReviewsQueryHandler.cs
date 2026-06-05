namespace eMechanic.Application.Workshop.Reviews.Features.Get.All;

using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class GetWorkshopReviewsQueryHandler : IResultQueryHandler<GetWorkshopReviewsQuery, PaginationResult<WorkshopReviewResponse>>
{
    private readonly IWorkshopReviewRepository _reviewRepository;

    public GetWorkshopReviewsQueryHandler(IWorkshopReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<PaginationResult<WorkshopReviewResponse>, Error>> Handle(
        GetWorkshopReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetForWorkshopPaginatedAsync(
            request.WorkshopId,
            request.PaginationParameters,
            cancellationToken);

        return reviews.MapToDto(x => new WorkshopReviewResponse(
            x.Id,
            x.WorkshopId,
            x.UserId,
            x.Rating.Value,
            x.Comment?.Value,
            x.CreatedAt,
            x.UpdatedAt));
    }
}

