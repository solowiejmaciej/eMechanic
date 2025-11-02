namespace eMechanic.API.Features.Vehicle.Get;

using eMechanic.API.Security;
using eMechanic.Application.Vehicle.Get;
using eMechanic.Application.Vehicle.Get.ById;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetVehicleByIdFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(VehiclePrefix.ENDPOINT + "/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetVehicleByIdQuery(id);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetVehicleById")
            .WithTags(VehiclePrefix.TAG)
            .Produces<VehicleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Gets a vehicle by its unique identifier.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
