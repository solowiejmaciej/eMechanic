namespace eMechanic.API.Features.User.GetCurrent;

using Common.Web;
using eMechanic.API.Features.User;
using eMechanic.API.Security;
using eMechanic.Application.Users.Get.Current;
using eMechanic.Common.Result;
using MediatR;

public sealed class GetCurrentUserFeature : IFeature
{
    public IResult MapError(Error error) => error.Code switch
    {
        _ => ErrorMapper.MapToHttpResult(error)
    };

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UserPrefix.ENDPOINT, async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCurrentUserQuery();
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetsCurrentUser")
            .WithTags(UserPrefix.TAG)
            .Produces<GetCurrentUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
