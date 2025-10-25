namespace eMechanic.API.Features.User.GetById;

using eMechanic.API.Features.User;
using eMechanic.Application.Users.GetById;
using eMechanic.Common.Result;
using MediatR;

public sealed class GetUserByIdFeature : IFeature
{
    public IResult MapError(Error error) => error.Code switch
    {
        _ => ErrorMapper.MapToHttpResult(error)
    };

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UserPrefix.ENDPOINT + "/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUserByIdQuery(id);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetUsersById")
            .WithTags(UserPrefix.TAG)
            .Produces<GetUsersByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
