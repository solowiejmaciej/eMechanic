namespace eMechanic.Application.Users.Services;

using eMechanic.Common.Result;

public interface IUserService
{
    Task<Result<Guid, Error>> CreateUserWithIdentityAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken);

    Task<Result<Success, Error>> UpdateUserWithIdentityAsync(
        Guid domainUserId,
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken);
}
