namespace eMechanic.API.Features.Repair.Get;

using Application.Repair.Features.Get.ById;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetRepairByIdFeature : IFeature //TODO Get all for both user and workshop
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(RepairPrefix.GET_BY_ID, async (
                [FromRoute] Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRepairByIdQuery(id);
                var result = await mediator.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetRepairById")
            .WithTags(RepairPrefix.TAG)
            .Produces<RepairResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Gets repair details by id.")
            .RequireAuthorization();
    }
}

