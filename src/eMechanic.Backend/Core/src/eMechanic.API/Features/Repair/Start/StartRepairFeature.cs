namespace eMechanic.API.Features.Repair.Start;

using Application.Repair.Features.Start;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class StartRepairFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(RepairPrefix.START, async (
                [FromRoute] Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new StartRepairCommand(id);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("StartRepair")
            .WithTags(RepairPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Starts a scheduled repair.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}

