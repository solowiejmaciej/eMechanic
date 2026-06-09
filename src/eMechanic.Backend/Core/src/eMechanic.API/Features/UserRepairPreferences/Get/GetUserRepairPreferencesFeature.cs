namespace eMechanic.API.Features.UserRepairPreferences.Get;

using Application.UserRepairPreferences.Features.Get;
using eMechanic.API.Features.UserRepairPreferences;
using eMechanic.API.Security;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetUserRepairPreferencesFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UserPreferencesPrefix.ENDPOINT + "/{userId:guid}", async (
                [FromRoute] Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUserRepairPreferencesQuery(userId);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetUserRepairPreferences")
            .WithTags(UserPreferencesPrefix.TAG)
            .Produces<UserRepairPreferencesResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithSummary("Returns repair preferences of a specific user for the active workshop")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}
