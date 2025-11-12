namespace eMechanic.API.Features.Vehicle.Create;

using Common.Web;
using eMechanic.API.Constans;
using eMechanic.API.Features.Vehicle;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Request;
using Security;

public sealed class CreateVehicleFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(VehiclePrefix.CREATE_ENDPOINT, async (
                CreateVehicleRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.ToCommand(), cancellationToken);

                return result.ToStatusCode(
                    vehicleId =>
                        Results.Created($"{WebApiConstans.CURRENT_API_VERSION}{VehiclePrefix.CREATE_ENDPOINT}/{vehicleId}",
                            new { VehicleId = vehicleId }),
                    MapError);
            })
            .WithName("CreateVehicle")
            .WithTags(VehiclePrefix.TAG)
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Creates a new vehicle.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
