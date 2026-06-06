namespace eMechanic.Application.Workshop.Reviews.Features.Upsert;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Domain.Workshop.Reviews;
using Repositories;
using eMechanic.Application.Workshop.Workshop.Repositories;

public sealed class UpsertWorkshopReviewCommandHandler : IResultCommandHandler<UpsertWorkshopReviewCommand, Guid>
{
    private readonly IUserContext _userContext;
    private readonly IWorkshopReviewRepository _reviewRepository;
    private readonly IWorkshopRepository _workshopRepository;

    public UpsertWorkshopReviewCommandHandler(
        IUserContext userContext,
        IWorkshopReviewRepository reviewRepository,
        IWorkshopRepository workshopRepository)
    {
        _userContext = userContext;
        _reviewRepository = reviewRepository;
        _workshopRepository = workshopRepository;
    }

    public async Task<Result<Guid, Error>> Handle(UpsertWorkshopReviewCommand request, CancellationToken cancellationToken)
    {
        var workshop = await _workshopRepository.GetByIdAsync(request.WorkshopId, cancellationToken);
        if (workshop is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Workshop with ID {request.WorkshopId} not found.");
        }

        var userId = _userContext.GetUserId();
        var existingReview = await _reviewRepository.GetForWorkshopByUserAsTrackingAsync(request.WorkshopId, userId, cancellationToken);

        if (existingReview is null)
        {
            var reviewResult = Review.Create(request.WorkshopId, userId, request.Rating, request.Comment);
            if (reviewResult.HasError())
            {
                return reviewResult.Error!;
            }

            var reviewId = await _reviewRepository.AddAsync(reviewResult.Value!, cancellationToken);
            await _reviewRepository.SaveChangesAsync(cancellationToken);
            return reviewId;
        }

        var updateResult = existingReview.Update(request.Rating, request.Comment);
        if (updateResult.HasError())
        {
            return updateResult.Error!;
        }

        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return existingReview.Id;
    }
}


