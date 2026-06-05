namespace eMechanic.API.Features.Workshop.Reviews.Delete;

using Application.Workshop.Reviews.Features.Delete;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class DeleteWorkshopReviewFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(WorkshopPrefix.DELETE_WORKSHOP_REVIEW_ENDPOINT, async (
                [FromRoute] Guid workshopId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new DeleteWorkshopReviewCommand(workshopId), cancellationToken);
                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("DeleteWorkshopReview")
            .WithTags(WorkshopPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Deletes the current user review for a workshop.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}


