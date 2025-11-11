namespace eMechanic.API.Features.Tokens.Create;

using Application.Token.Features.Create.Workshop;
using eMechanic.API.Features.Tokens.Create.Request;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Workshop;

public sealed class CreateWorkshopTokenFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(TokenPrefix.ENDPOINT + "/workshop",  async (
                CreateWorkshopTokenRequest tokenRequest,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(tokenRequest.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    Results.Ok,
                    MapError);
            })
            .WithTags(TokenPrefix.TAG)
            .Produces<CreateWorkshopTokenResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Create JWT token for workshop");
    }
}
