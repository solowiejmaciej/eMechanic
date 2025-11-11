namespace eMechanic.Application.Users.Features.Register;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Services;

public class RegisterUserHandler : IResultCommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserService _userService;

    public RegisterUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<Guid, Error>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService
            .CreateUserWithIdentityAsync(request.Email, request.Password, request.FirstName, request.LastName, cancellationToken);

        return result;
    }
}
