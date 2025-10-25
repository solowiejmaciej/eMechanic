namespace eMechanic.API.Features.User.Login;

using eMechanic.API.Features.User;
using eMechanic.Application.Users.Login;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class LoginUserFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UserPrefix.ENDPOINT + "/login",  async (
                LoginUserCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);

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
