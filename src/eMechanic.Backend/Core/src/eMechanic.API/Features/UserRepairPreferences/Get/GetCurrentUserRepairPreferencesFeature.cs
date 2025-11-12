namespace eMechanic.API.Features.UserRepairPreferences.Get;

using Application.UserRepairPreferences.Features.Get;
using eMechanic.API.Features.UserRepairPreferences;
using eMechanic.API.Security;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;

public sealed class GetCurrentUserRepairPreferencesFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UserPreferencesPrefix.GET_ENDPOINT, async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCurrentUserRepairPreferencesQuery();
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetCurrentUserRepairPreferences")
            .WithTags(UserPreferencesPrefix.TAG)
            .Produces<UserRepairPreferencesResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Returns current user repair preferences")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
