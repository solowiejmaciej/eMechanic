namespace eMechanic.API.Features.Workshop.Login;

using Application.Workshop.Login;
using eMechanic.Application.Users.Login;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class LoginWorkshopFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(WorkshopPrefix.ENDPOINT + "/login",  async (
                LoginWorkshopCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);

                return result.ToStatusCode(
                    Results.Ok,
                    MapError);
            })
            .WithTags(WorkshopPrefix.TAG)
            .Produces<LoginUserResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Logs in a user and returns a JWT token.");
    }
}
