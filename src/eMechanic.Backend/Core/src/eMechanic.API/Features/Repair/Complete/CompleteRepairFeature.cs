namespace eMechanic.API.Features.Repair.Complete;

using Application.Repair.Features.Complete;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class CompleteRepairFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(RepairPrefix.COMPLETE, async (
                [FromRoute] Guid id,
                [FromBody] CompleteRepairRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CompleteRepairCommand(id, request.Amount, request.Currency);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("CompleteRepair")
            .WithTags(RepairPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Completes an in-progress repair with final cost.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}

public sealed record CompleteRepairRequest(decimal Amount, string Currency);

