namespace eMechanic.API.Features.RepairRequest.Get;

using Application.RepairRequest.Features.Get;
using Application.RepairRequest.Features.Get.ById;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class GetRepairRequestByIdFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(RepairRequestPrefix.GET_BY_ID, async (
                [FromRoute] Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRepairRequestByIdQuery(id);
                var result = await mediator.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetRepairRequestById")
            .WithTags(RepairRequestPrefix.TAG)
            .Produces<RepairRequestResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Gets repair request details by ID.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER_OR_WORKSHOP);
    }
}
