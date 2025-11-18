
namespace eMechanic.API.Features.RepairRequest.ProvideEstimation;

using Application.RepairRequest.Features.ProvideEstimation;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class ProvideEstimationFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(RepairRequestPrefix.PROVIDE_ESTIMATION, async (
                [FromRoute] Guid id,
                [FromBody] ProvideRepairEstimationRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new ProvideRepairEstimationCommand(id, request.Diagnosis, request.Cost, request.Currency);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("ProvideRepairEstimation")
            .WithTags(RepairRequestPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Provides an estimation for a repair request.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}

public record ProvideRepairEstimationRequest(string Diagnosis, decimal Cost, string Currency);
