namespace eMechanic.API.Features.Workshop.Document.Delete;

using eMechanic.API.Features.Workshop.Document;
using eMechanic.API.Security;
using eMechanic.Application.Workshop.Document.Features.Delete;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class DeleteWorkshopDocumentFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(WorkshopDocumentPrefix.DELETE, async (
                [FromRoute] Guid documentId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteWorkshopDocumentCommand(documentId);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(_ => Results.NoContent(), MapError);
            })
            .WithName("DeleteWorkshopDocument")
            .WithTags(WorkshopDocumentPrefix.TAG)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Deletes workshop document")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP);
    }
}
