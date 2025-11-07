namespace eMechanic.API.Features.Vehicle.Delete;

using Application.Vehicle.Features.Delete;
using Common.Web;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class DeleteVehicleFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(VehiclePrefix.ENDPOINT + "/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteVehicleCommand(id);
                var result = await mediator.Send(command, cancellationToken);

                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("DeleteVehicle")
            .WithTags(VehiclePrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Deletes a vehicle by its unique identifier.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
