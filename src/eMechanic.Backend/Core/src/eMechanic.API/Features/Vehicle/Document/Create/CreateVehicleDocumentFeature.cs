namespace eMechanic.API.Features.Vehicle.Document.Create;

using eMechanic.API.Constans;
using eMechanic.API.Security;
using eMechanic.Application.VehicleDocument.Features.Create;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using eMechanic.Domain.VehicleDocument.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class CreateVehicleDocumentFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(VehicleDocumentPrefix.ENDPOINT, async (
                [FromRoute] Guid vehicleId,
                IFormFile file,
                [FromForm] EVehicleDocumentType documentType,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new AddVehicleDocumentCommand(vehicleId, file, documentType);
                var result = await mediator.Send(command, cancellationToken);

                return result.ToStatusCode(
                    documentId =>
                    {
                        var locationUrl = VehicleDocumentPrefix.GET_BY_ID
                            .Replace("{vehicleId:guid}", vehicleId.ToString())
                            .Replace("{documentId:guid}", documentId.ToString());

                        var fullUrl = $"/{WebApiConstans.CURRENT_API_VERSION}{locationUrl}";

                        return Results.Created(fullUrl, new { DocumentId = documentId });
                    },
                    MapError);
            })
            .WithName("CreateVehicleDocument")
            .WithTags(VehicleDocumentPrefix.TAG)
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Uploads new vehicle document")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER)
            .DisableAntiforgery();
    }
}
