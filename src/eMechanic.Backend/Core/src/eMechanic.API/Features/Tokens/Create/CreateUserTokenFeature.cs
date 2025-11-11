namespace eMechanic.API.Features.Tokens.Create;

using Application.Token.Features.Create.User;
using eMechanic.API.Features.Tokens;
using eMechanic.API.Features.Tokens.Create.Request;
using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class CreateUserTokenFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(TokenPrefix.ENDPOINT + "/user",  async (
                CreateUserTokenRequest tokenRequest,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(tokenRequest.MapToCommand(), cancellationToken);

                return result.ToStatusCode(
                    Results.Ok,
                    MapError);
            })
            .WithTags(TokenPrefix.TAG)
            .Produces<CreateUserTokenResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Create JWT token for workshop");
    }
}
