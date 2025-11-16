namespace eMechanic.API.Features.Vehicle.Vehicle.Get;

using eMechanic.API.Security;
using eMechanic.Application.Vehicle.Features.Get;
using eMechanic.Application.Vehicle.Features.Get.All;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetAllVehiclesFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(VehiclePrefix.GET_ALL, async (
                [AsParameters] PaginationParameters paginationParameters,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetVehiclesQuery(paginationParameters);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetVehicles")
            .WithTags(VehiclePrefix.TAG)
            .Produces<PaginationResult<VehicleResponse>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Gets a vehicle by its unique identifier.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
