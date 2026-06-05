namespace eMechanic.API.Features.Workshop.Reviews.Get;

using Application.Workshop.Reviews.Features.Get;
using Application.Workshop.Reviews.Features.Get.Stats;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetWorkshopReviewStatsFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(WorkshopPrefix.GET_WORKSHOP_REVIEW_STATS_ENDPOINT, async (
                [FromRoute] Guid workshopId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetWorkshopReviewStatsQuery(workshopId), cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetWorkshopReviewStats")
            .WithTags(WorkshopPrefix.TAG)
            .Produces<WorkshopReviewStatsResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Gets workshop review statistics (average rating and count).");
    }
}

