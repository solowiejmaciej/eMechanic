namespace eMechanic.API.Features.Repair.Get;

using Application.Repair.Features.Get.ById;
using Application.Repair.Features.Get.ById.ForWorkshop;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class GetWorkshopRepairByIdFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(RepairPrefix.GET_BY_ID_FOR_WORKSHOP, async (
                [FromRoute] Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetWorkshopRepairByIdQuery(id);
                var result = await mediator.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetWorkshopRepairById")
            .WithTags(RepairPrefix.TAG)
            .Produces<RepairResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Gets repair details by id for the authenticated workshop.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}

