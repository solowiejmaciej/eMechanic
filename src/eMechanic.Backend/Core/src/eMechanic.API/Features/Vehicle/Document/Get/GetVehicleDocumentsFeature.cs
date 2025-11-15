namespace eMechanic.API.Features.Vehicle.Document.Get;

using Application.VehicleDocument.Features.Get;
using eMechanic.API.Features.Vehicle.Document;
using eMechanic.API.Security;
using eMechanic.Application.VehicleDocument.Features.Get.All;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetVehicleDocumentsFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(VehicleDocumentPrefix.ENDPOINT, async (
                [FromRoute] Guid vehicleId,
                [AsParameters] PaginationParameters pagination,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetVehicleDocumentsQuery(vehicleId, pagination);
                var result = await mediator.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetVehicleDocuments")
            .WithTags(VehicleDocumentPrefix.TAG)
            .Produces<PaginationResult<VehicleDocumentResponse>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Gets vehicle documents list")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
