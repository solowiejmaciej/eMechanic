namespace eMechanic.Application.Users.Features.Get.Current;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.User;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public sealed class GetCurrentUserHandler : IResultQueryHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _context;

    public GetCurrentUserHandler(
        IUserRepository userRepository,
        IUserContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Result<GetCurrentUserResponse, Error>> Handle(GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _context.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            return new Error(EErrorCode.NotFoundError, "User not found");
        }

        var response = new GetCurrentUserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.CreatedAt);

        return response;
    }
}
