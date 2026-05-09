namespace eMechanic.API.Features.RepairRequest.Get;

using Application.RepairRequest.Features.Get;
using Application.RepairRequest.Features.Get.ForUser;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class GetRepairRequestsForUserVehicleFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(RepairRequestPrefix.GET_BY_VEHICLE_ID, async (
                [FromRoute] Guid vehicleId,
                [AsParameters] PaginationParameters pagination,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRepairRequestsForUserVehicleQuery(vehicleId, pagination);
                var result = await mediator.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetRepairRequestsForVehicle")
            .WithTags(RepairRequestPrefix.TAG)
            .Produces<PaginationResult<RepairRequestResponse>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Gets paginated repair requests for a specific vehicle owned by the authenticated user.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
