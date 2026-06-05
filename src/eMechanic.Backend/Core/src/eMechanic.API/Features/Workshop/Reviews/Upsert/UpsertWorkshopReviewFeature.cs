namespace eMechanic.API.Features.Workshop.Reviews.Upsert;

using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;
using Security;

public sealed class UpsertWorkshopReviewFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(WorkshopPrefix.UPSERT_WORKSHOP_REVIEW_ENDPOINT, async (
                [FromRoute] Guid workshopId,
                [FromBody] UpsertWorkshopReviewRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.MapToCommand(workshopId), cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("UpsertWorkshopReview")
            .WithTags(WorkshopPrefix.TAG)
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Creates or updates the current user review for a workshop.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}

