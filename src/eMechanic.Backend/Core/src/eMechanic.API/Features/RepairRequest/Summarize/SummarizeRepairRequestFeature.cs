namespace eMechanic.API.Features.RepairRequest.Summarize;

using Application.RepairRequest.Features.Summarize;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public class SummarizeRepairRequestFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(RepairRequestPrefix.SUMMARIZE, async (
                [FromRoute] Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new SummarizeRepairRequestCommand(id), cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithTags(RepairRequestPrefix.TAG)
            .WithName("Summarize Repair Request")
            .WithSummary("Summarizes repair request with AI")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}
