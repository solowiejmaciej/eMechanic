namespace eMechanic.API.Features.Workshop.Document.Create;

using API.Security;
using Application.Workshop.Document.Features.Create;
using Common.Result;
using Common.Web;
using Domain.Workshop.Documents.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class CreateWorkshopDocumentFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"{WorkshopDocumentPrefix.ENDPOINT}", async (
                IFormFile file,
                [FromForm] EWorkshopDocumentType documentType,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new AddWorkshopDocumentCommand(file, documentType);
                var result = await mediator.Send(command, cancellationToken);

                return result.ToStatusCode(
                    uri => Results.Created(uri, new { PublicUrl = uri.ToString() }),
                    MapError);
            })
            .WithName("CreateWorkshopDocument")
            .WithTags("Workshop Documents")
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Uploads a public workshop document (e.g. Logo, Gallery Image).")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_WORKSHOP)
            .DisableAntiforgery();
    }
}
