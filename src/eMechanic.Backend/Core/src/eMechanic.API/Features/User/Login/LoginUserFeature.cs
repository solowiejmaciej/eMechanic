namespace eMechanic.API.Features.User.Login;

using Application.Users.Features.Login;
using Common.Web;
using eMechanic.API.Features.User;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class LoginUserFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UserPrefix.ENDPOINT + "/login",  async (
                LoginUserRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    Results.Ok,
                    MapError);
            })
            .WithTags(UserPrefix.TAG)
            .Produces<LoginUserResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Logs in a user and returns a JWT token.");
    }
}
