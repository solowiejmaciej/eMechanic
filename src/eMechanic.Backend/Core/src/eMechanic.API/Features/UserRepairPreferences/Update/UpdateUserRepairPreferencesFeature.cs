namespace eMechanic.API.Features.UserRepairPreferences.Update;

using eMechanic.API.Features.UserRepairPreferences;
using eMechanic.API.Security;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;

public sealed class UpdateUserRepairPreferencesFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(UserPreferencesPrefix.ENDPOINT, async (
                [FromBody] UpdateUserRepairPreferencesRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                var result = await mediator.Send(command, cancellationToken);

                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("UpdateCurrentUserRepairPreferences")
            .WithTags(UserPreferencesPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Updates repair preferences for current user")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
