namespace eMechanic.Application.Workshop.Reviews.Features.Delete;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class DeleteWorkshopReviewCommandHandler : IResultCommandHandler<DeleteWorkshopReviewCommand, bool>
{
    private readonly IUserContext _userContext;
    private readonly IWorkshopReviewRepository _reviewRepository;

    public DeleteWorkshopReviewCommandHandler(IUserContext userContext, IWorkshopReviewRepository reviewRepository)
    {
        _userContext = userContext;
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<bool, Error>> Handle(DeleteWorkshopReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();
        var existingReview = await _reviewRepository.GetForWorkshopByUserAsTrackingAsync(request.WorkshopId, userId, cancellationToken);

        if (existingReview is null)
        {
            return new Error(EErrorCode.NotFoundError, "Workshop review was not found for the current user.");
        }

        var deleteResult = existingReview.Delete();
        if (deleteResult.HasError())
        {
            return deleteResult.Error!;
        }

        await _reviewRepository.DeleteAsync(existingReview, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}

