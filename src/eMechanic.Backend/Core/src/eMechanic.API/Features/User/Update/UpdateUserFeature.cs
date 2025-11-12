namespace eMechanic.API.Features.User.Update;

using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;
using Security;

public sealed class UpdateUserFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(UserPrefix.UPDATE_CURRENT_USER_ENDPOINT, async (
                [FromBody] UpdateUserRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    _ => Results.NoContent(),
                    MapError);
            })
            .WithName("UpdateCurrentUser")
            .WithTags(UserPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Updates details for the currently authenticated user.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
