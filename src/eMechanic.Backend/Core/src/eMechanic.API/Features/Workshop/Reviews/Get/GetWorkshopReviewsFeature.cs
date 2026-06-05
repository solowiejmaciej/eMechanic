namespace eMechanic.API.Features.Workshop.Reviews.Get;

using Application.Workshop.Reviews.Features.Get;
using Application.Workshop.Reviews.Features.Get.All;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetWorkshopReviewsFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(WorkshopPrefix.GET_WORKSHOP_REVIEWS_ENDPOINT, async (
                [FromRoute] Guid workshopId,
                [AsParameters] PaginationParameters paginationParameters,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetWorkshopReviewsQuery(workshopId, paginationParameters), cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetWorkshopReviews")
            .WithTags(WorkshopPrefix.TAG)
            .Produces<PaginationResult<WorkshopReviewResponse>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Gets workshop reviews with pagination and search support.");
    }
}

