namespace eMechanic.Application.Users.Features.Create;

using eMechanic.Application.Users.Services;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class CreateUserHandler : IResultCommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserService _userService;

    public CreateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<Guid, Error>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService
            .CreateUserWithIdentityAsync(request.Email, request.Password, request.FirstName, request.LastName, cancellationToken);

        return result;
    }
}
