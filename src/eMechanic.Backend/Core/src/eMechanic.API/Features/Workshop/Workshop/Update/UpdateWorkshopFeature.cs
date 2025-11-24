namespace eMechanic.API.Features.Workshop.Workshop.Update;

using eMechanic.API.Security;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;

public sealed class UpdateWorkshopFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(WorkshopPrefix.UPDATE_ENDPOINT, async (
                [FromBody] UpdateWorkshopRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    _ => Results.NoContent(),
                    MapError);
            })
            .WithName("UpdateCurrentWorkshop")
            .WithTags(WorkshopPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates details for the currently authenticated workshop.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}
