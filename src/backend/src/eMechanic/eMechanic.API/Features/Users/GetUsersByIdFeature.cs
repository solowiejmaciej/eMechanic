namespace eMechanic.API.Features.Users;

using Common.CQRS;
using Common.Result;
using MediatR;

public record GetUsersByIdQuery(Guid Id) : IResultQuery<GetUsersByIdResponse>;
public record GetUsersByIdResponse(Guid Id, string Name, string Email);

public sealed class GetUsersByIdFeature : IFeature
{
    public IResult MapError(Error error) => error.Code switch
    {
        EErrorCode.NotFound => Results.NotFound(error),
        _ => Results.Problem(
            title: error.Code.ToString(),
            detail: error.Message,
            statusCode: StatusCodes.Status500InternalServerError)
    };

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUsersByIdQuery(id);
                var result = await mediator.Send(query, cancellationToken);

                return result.ToStatusCode(Results.Ok, MapError);
            })
            .WithName("GetUsersById")
            .WithTags("Users")
            .Produces<GetUsersByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}

public sealed class GetUsersByIdHandler : IResultQueryHandler<GetUsersByIdQuery, GetUsersByIdResponse>
{
    public async Task<Result<GetUsersByIdResponse, Error>> Handle(GetUsersByIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetUsersByIdResponse(request.Id, "John Doe", "johndoe@gmail.com");

        await Task.Delay(1000, cancellationToken);

        return new Error(EErrorCode.NotFound);

        //return response;
    }
}
