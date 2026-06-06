namespace eMechanic.Application.Users.Services;

using eMechanic.Common.Result;

public interface IUserService
{
    Task<Result<(Guid UserId, Guid IdentityId), Error>> CreateUserWithIdentityAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string? phoneNumber = null,
        string? providerName = null,
        string? providerKey = null,
        CancellationToken cancellationToken = default);

    Task<Result<Success, Error>> UpdateUserWithIdentityAsync(
        Guid domainUserId,
        string email,
        string firstName,
        string lastName,
        string? phoneNumber,
        CancellationToken cancellationToken);
}
