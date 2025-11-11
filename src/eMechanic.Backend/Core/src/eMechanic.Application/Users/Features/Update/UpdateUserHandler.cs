namespace eMechanic.Application.Users.Features.Update;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Services;

public class UpdateUserHandler : IResultCommandHandler<UpdateUserCommand, Success>
{
    private readonly IUserContext _userContext;
    private readonly IUserService _userService;

    public UpdateUserHandler(
        IUserContext userContext,
        IUserService userService)
    {
        _userContext = userContext;
        _userService = userService;
    }

    public async Task<Result<Success, Error>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var domainUserId = _userContext.GetUserId();

        return await _userService.UpdateUserWithIdentityAsync(
            domainUserId,
            request.Email,
            request.FirstName,
            request.LastName,
            cancellationToken);
    }
}
