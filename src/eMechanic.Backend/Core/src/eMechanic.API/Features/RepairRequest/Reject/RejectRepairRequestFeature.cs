
namespace eMechanic.API.Features.RepairRequest.Reject;

using Application.RepairRequest.Features.Reject;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;
using Security;

public sealed class RejectRepairRequestFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(RepairRequestPrefix.REJECT_ESTIMATION, async (
                [FromRoute] Guid id,
                RejectRepairRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new RejectRepairEstimationCommand(id, request.Reason);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("RejectRepairEstimation")
            .WithTags(RepairRequestPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Rejects a repair estimation.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
