namespace eMechanic.API.Features.Workshop.Register;

using Common.Result;
using Common.Web;
using Constans;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class RegisterWorkshopFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(WorkshopPrefix.ENDPOINT + "/register", async (
                RegisterWorkshopRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    id => Results.Created($"{WebApiConstans.CURRENT_API_VERSION}/WorkshopPrefix.ENDPOINT/{id}", new { WorkshopId = id }),
                    MapError);
            })
            .WithName("RegisterWorkshop")
            .WithTags(WorkshopPrefix.TAG)
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Registers a new workshop in the system.");
    }
}
