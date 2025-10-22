namespace eMechanic.Application.Users.GetById;

using Common.CQRS;
using Common.Result;

public sealed class GetUserByIdHandler : IResultQueryHandler<GetUserByIdQuery, GetUsersByIdResponse>
{
    public async Task<Result<GetUsersByIdResponse, Error>> Handle(GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetUsersByIdResponse(request.Id, "John Doe", "johndoe@gmail.com");

        await Task.Delay(1000, cancellationToken);

        return new Error(EErrorCode.NotFoundError);

        //return response;
    }
}
