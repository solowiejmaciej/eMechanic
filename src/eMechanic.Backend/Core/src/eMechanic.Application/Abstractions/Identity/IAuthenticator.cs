namespace eMechanic.Application.Abstractions.Identity;

using Application.Identity;
using Common.Result;

public interface IAuthenticator
{
    Task<Result<AuthenticatedIdentity, Error>> AuthenticateAsync(
        string email,
        string password,
        EIdentityType expectedType);
}
