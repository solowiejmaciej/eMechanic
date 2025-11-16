namespace eMechanic.API.Features.Vehicle.Vehicle.Create;

using eMechanic.API.Constans;
using eMechanic.API.Features.Vehicle;
using eMechanic.API.Security;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;

public sealed class CreateVehicleFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(VehiclePrefix.CREATE, async (
                CreateVehicleRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.ToCommand(), cancellationToken);

                return result.ToStatusCode(
                    vehicleId =>
                        Results.Created($"{WebApiConstans.CURRENT_API_VERSION}{VehiclePrefix.CREATE}/{vehicleId}",
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
