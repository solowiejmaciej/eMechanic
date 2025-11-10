namespace eMechanic.Application.Users.Features.Register;

using eMechanic.Application.Abstractions.User;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class RegisterUserHandler : IResultCommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserCreatorService _userCreatorService;

    public RegisterUserHandler(IUserCreatorService userCreatorService)
    {
        _userCreatorService = userCreatorService;
    }

    public async Task<Result<Guid, Error>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userCreatorService
            .CreateUserWithIdentityAsync(request.Email, request.Password, request.FirstName, request.LastName, cancellationToken);

        return result;
    }
}
