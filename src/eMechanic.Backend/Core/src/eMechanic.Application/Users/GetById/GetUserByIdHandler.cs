namespace eMechanic.Application.Users.GetById;

using Abstractions.User;
using Common.CQRS;
using Common.Result;

public sealed class GetUserByIdHandler : IResultQueryHandler<GetUserByIdQuery, GetUsersByIdResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUsersByIdResponse, Error>> Handle(GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return new Error(EErrorCode.NotFoundError, "User not found");
        }

        var response = new GetUsersByIdResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.CreatedAt);

        return response;
    }
}
