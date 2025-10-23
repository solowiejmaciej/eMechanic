namespace eMechanic.Application.Abstractions.User;

using Common.Result;
using Domain.Users;

public interface IUserCreatorService
{
    Task<Result<Guid, Error>> CreateUserWithIdentityAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken);
}
