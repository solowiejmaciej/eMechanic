namespace eMechanic.API.Features.Tokens.Create.External;

using Application.Token.Features.Create.User;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;

public class CreateUserTokenFromGoogleFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(TokenPrefix.CREATE_EXTERNAL_USER_TOKEN_ENDPOINT, async (
                CreateGoogleTokenRequest tokenRequest,
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
            .WithSummary("Create JWT token for user via google");
    }
}
