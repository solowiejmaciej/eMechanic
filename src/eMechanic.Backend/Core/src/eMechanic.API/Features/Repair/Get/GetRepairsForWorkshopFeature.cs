namespace eMechanic.API.Features.Repair.Get;

using Application.Repair.Features.Get;
using Application.Repair.Features.Get.ForWorkshop;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class GetRepairsForWorkshopFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(RepairPrefix.GET_FOR_WORKSHOP, async (
                [AsParameters] PaginationParameters pagination,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetRepairsForWorkshopQuery(pagination);
                var result = await mediator.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetRepairsForWorkshop")
            .WithTags(RepairPrefix.TAG)
            .Produces<PaginationResult<RepairListItemResponse>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Lists paginated repairs for authenticated workshop.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}

