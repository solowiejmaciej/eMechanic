namespace eMechanic.NotificationService.Features.Test.Get;

using Common.CQRS;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class LoginUserFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(TestPrefix.ENDPOINT, async(
            IMediator mediator,
            SendTestNotificationComand command,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken );

                return result.ToStatusCode(
                    id => Results.Ok(), MapError);
            })
            .WithName("RegisterUser")
            .WithTags(TestPrefix.TAG)
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Registers a new user in the system.");
    }


}
