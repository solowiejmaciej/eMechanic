
using eMechanic.Application.RepairRequest.Features.Get;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eMechanic.API.Features.RepairRequest.Get;

public class GetRepairRequestByIdFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{RepairRequestPrefix.PREFIX}/{{id}}", async (
            [FromRoute] Guid id,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetRepairRequestByIdQuery(id));
            return result.ToStatusCode(Results.Ok, MapError);
        }).RequireAuthorization();
    }
}
