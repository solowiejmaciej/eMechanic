namespace eMechanic.API.Features.Workshop.Get;

using Application.Workshop.Get;
using Application.Workshop.Get.All;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetAllWorkshopsFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(WorkshopPrefix.ENDPOINT + "", async (
                [AsParameters] PaginationParameters paginationParameters,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetWorkshopsQuery(paginationParameters);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetWorkshops")
            .WithTags(WorkshopPrefix.TAG)
            .Produces<PaginationResult<WorkshopResponse>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Gets all workshops paginated");
    }}
