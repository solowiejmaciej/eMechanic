namespace eMechanic.API.Features.User.Register;

using Common.Web;
using eMechanic.API.Constans;
using eMechanic.API.Features.User;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;

public sealed class RegisterUserFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UserPrefix.ENDPOINT +"/register", async (
                RegisterUserRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    id => Results.Created($"{WebApiConstans.CURRENT_API_VERSION}{UserPrefix.ENDPOINT}/{id}", new { UserId = id }),
                    MapError);
            })
            .WithName("RegisterUser")
            .WithTags(UserPrefix.TAG)
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Registers a new user in the system.");
    }
}
