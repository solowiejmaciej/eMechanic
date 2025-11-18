
namespace eMechanic.API.Features.RepairRequest.Create;

using Application.RepairRequest.Features.Create;
using Common.Result;
using MediatR;
using Security;
using Common.Web;
using Microsoft.AspNetCore.Mvc;

public sealed class CreateRepairRequestFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(RepairRequestPrefix.CREATE, async (
                CreateRepairRequestCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);

                return result.ToStatusCode(
                    repairRequestId => Results.Created(
                        $"{RepairRequestPrefix.CREATE}/{repairRequestId}",
                        new { RepairRequestId = repairRequestId }),
                    MapError);
            })
            .WithName("CreateRepairRequest")
            .WithTags(RepairRequestPrefix.TAG)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Creates a new repair request.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
