namespace eMechanic.API.Features.Workshop.Document.Get;

using Application.Workshop.Document.Features.Get;
using eMechanic.API.Features.Workshop.Document;
using eMechanic.API.Security;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class DownloadWorkshopDocumentFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(WorkshopDocumentPrefix.DOWNLOAD, async (
                [FromRoute] Guid workshopId,
                [FromRoute] Guid documentId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetWorkshopDocumentFileQuery(workshopId, documentId);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(
                    fileResult => Results.Stream(
                        fileResult.Content,
                        fileResult.ContentType,
                        fileResult.FileName),
                    MapError);
            })
            .WithName("DownloadWorkshopDocument")
            .WithTags(WorkshopDocumentPrefix.TAG)
            .Produces(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Downloads workshop document")
            .AllowAnonymous();
    }
}
