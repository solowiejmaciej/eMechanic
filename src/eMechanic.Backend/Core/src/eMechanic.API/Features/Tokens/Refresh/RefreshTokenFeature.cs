namespace eMechanic.API.Features.Tokens.Refresh;

using Application.Token.Features.Refresh;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Request;

public sealed class RefreshTokenFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(TokenPrefix.ENDPOINT + "/refresh",  async (
                RefreshTokenRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var test = request.MapToCommand();
                var result = await mediator.Send(test, cancellationToken);

                return result.ToStatusCode(
                    Results.Ok,
                    MapError);
            })
            .WithTags(TokenPrefix.TAG)
            .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Refreshes jwt token based on refreshToken");
    }
}
