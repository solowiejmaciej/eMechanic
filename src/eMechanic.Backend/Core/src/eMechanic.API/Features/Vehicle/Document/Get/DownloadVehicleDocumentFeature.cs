namespace eMechanic.API.Features.Vehicle.Document.Get;

using eMechanic.API.Features.Vehicle.Document;
using eMechanic.API.Security;
using eMechanic.Application.VehicleDocument.Features.Get.ById;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class DownloadVehicleDocumentFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(VehicleDocumentPrefix.DOWNLOAD, async (
                [FromRoute] Guid vehicleId,
                [FromRoute] Guid documentId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetVehicleDocumentFileQuery(vehicleId, documentId);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(
                    fileResult => Results.Stream(
                        fileResult.Content,
                        fileResult.ContentType,
                        fileResult.FileName),
                    MapError);
            })
            .WithName("DownloadVehicleDocument")
            .WithTags(VehicleDocumentPrefix.TAG)
            .Produces(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Downloads vehicle document")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
