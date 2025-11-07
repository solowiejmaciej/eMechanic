namespace eMechanic.NotificationService.Features.Test.Get;

using eMechanic.Common.Result;
using eMechanic.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public sealed class LoginUserFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(TestPrefix.ENDPOINT, () => Task.FromResult(TypedResults.Ok()))
            .WithTags(TestPrefix.TAG)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Testing endpoint will always return 200");
    }
}
