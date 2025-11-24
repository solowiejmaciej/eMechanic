namespace eMechanic.API.Features.Workshop.Document.Get;

using API.Features.Workshop;
using Application.Workshop.Document.Features.Get;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class GetWorkshopDocumentsFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(WorkshopDocumentPrefix.GET_ALL, async (
                Guid workshopId,
                [AsParameters] PaginationParameters pagination,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetWorkshopDocumentsQuery(workshopId, pagination);
                var result = await mediator.Send(query, cancellationToken);
                return result.ToStatusCode(Results.Ok, MapError);
            })
            .Produces<PaginationResult<WorkshopDocumentResponse>>(StatusCodes.Status200OK)
            .WithName("GetWorkshopDocuments")
            .WithTags(WorkshopDocumentPrefix.TAG)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get public documents (gallery, logo) for a specific workshop.")
            .AllowAnonymous();
    }
}
