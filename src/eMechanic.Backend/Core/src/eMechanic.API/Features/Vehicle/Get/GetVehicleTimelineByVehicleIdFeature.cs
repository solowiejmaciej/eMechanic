namespace eMechanic.API.Features.Vehicle.Get;

using Application.Vehicle.Features.Get;
using Application.Vehicle.Features.Get.Timeline;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class GetVehicleTimelineByVehicleIdFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(VehiclePrefix.GET_TIMELINE_ENDPOINT, async (
                Guid id,
                [AsParameters] PaginationParameters paginationParameters,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetVehicleTimelineByVehicleIdQuery(id, paginationParameters);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetVehicleTimelineByVehicleId")
            .WithTags(VehiclePrefix.TAG)
            .Produces<PaginationResult<VehicleTimelineResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Gets a vehicle's timeline by vehicleId")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
