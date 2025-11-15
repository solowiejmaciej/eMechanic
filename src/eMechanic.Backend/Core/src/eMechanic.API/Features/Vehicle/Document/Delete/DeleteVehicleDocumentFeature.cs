namespace eMechanic.API.Features.Vehicle.Document.Delete;

using eMechanic.API.Features.Vehicle.Document;
using eMechanic.API.Security;
using eMechanic.Application.VehicleDocument.Features.Delete;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class DeleteVehicleDocumentFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(VehicleDocumentPrefix.DELETE, async (
                [FromRoute] Guid vehicleId,
                [FromRoute] Guid documentId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteVehicleDocumentCommand(vehicleId, documentId);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("DeleteVehicleDocument")
            .WithTags(VehicleDocumentPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Deletes vehicle document")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
