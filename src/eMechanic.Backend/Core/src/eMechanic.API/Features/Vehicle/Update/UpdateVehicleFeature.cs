namespace eMechanic.API.Features.Vehicle.Update;

using Common.Web;
using eMechanic.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;
using Security;

public sealed class UpdateVehicleFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(VehiclePrefix.ENDPOINT + "/{id:guid}", async (
                Guid id,
                [FromBody] UpdateVehicleRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request.ToCommand(id), cancellationToken);

                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("UpdateVehicle")
            .WithTags(VehiclePrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Updates an existing vehicle.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
