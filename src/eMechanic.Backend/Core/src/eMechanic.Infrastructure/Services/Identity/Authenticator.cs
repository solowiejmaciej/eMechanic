namespace eMechanic.Infrastructure.Services.Identity;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Common.Result;
using eMechanic.Common.Result.Fields;
using Microsoft.AspNetCore.Identity;

internal sealed class Authenticator : IAuthenticator
{
    private readonly UserManager<Infrastructure.Identity.Identity> _userManager;

    public Authenticator(UserManager<Infrastructure.Identity.Identity> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<AuthenticatedIdentity, Error>> AuthenticateAsync(
        string email, string password, EIdentityType expectedType)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);

        if (identityUser is null)
        {
            return Error.Validation(EField.General, "Invalid email or password.");
        }

        if (identityUser.Type != expectedType)
        {
            return Error.Validation(EField.General, "Invalid email or password.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(identityUser, password);

        if (!passwordValid)
        {
            return Error.Validation(EField.General, "Invalid email or password.");
        }

        var authenticatedUser = new AuthenticatedIdentity(
            identityUser.Id,
            identityUser.Email!,
            identityUser.Type
        );

        return authenticatedUser;
    }
}
