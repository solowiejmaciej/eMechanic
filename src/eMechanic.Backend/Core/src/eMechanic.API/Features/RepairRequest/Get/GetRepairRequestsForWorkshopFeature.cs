namespace eMechanic.API.Features.RepairRequest.Get;

using Application.RepairRequest.Features.Get;
using Application.RepairRequest.Features.Get.ForWorkshop;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class GetRepairRequestsForWorkshopFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(RepairRequestPrefix.GET_BY_WORKSHOP_ID, async (
                [AsParameters] PaginationParameters pagination,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRepairRequestsForWorkshopQuery(pagination);
                var result = await sender.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetRepairRequestsForWorkshop")
            .WithTags(RepairRequestPrefix.TAG)
            .Produces<PaginationResult<RepairRequestResponse>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Gets paginated repair requests for the currently authenticated workshop.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}
