namespace eMechanic.API.Features.Workshop.Create;

using eMechanic.API.Constans;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class CreateWorkshopFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(WorkshopPrefix.ENDPOINT, async (
                CreateWorkshopRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    id => Results.Created($"{WebApiConstans.CURRENT_API_VERSION}/{WorkshopPrefix.ENDPOINT}/{id}", new { WorkshopId = id }),
                    MapError);
            })
            .WithName("CreateWorkshop")
            .WithTags(WorkshopPrefix.TAG)
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new workshop in the system.");
    }
}
