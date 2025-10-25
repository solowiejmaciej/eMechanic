namespace eMechanic.Infrastructure.Services.Identity;

using Application.Abstractions.User;
using Application.Abstractions.Workshop;
using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Common.Result;
using eMechanic.Common.Result.Fields;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

internal sealed class Authenticator : IAuthenticator
{
    private readonly UserManager<Identity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IWorkshopRepository _workshopRepository;

    public Authenticator(
        UserManager<Identity> userManager,
        IUserRepository userRepository,
        IWorkshopRepository workshopRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _workshopRepository = workshopRepository;
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

        var domainEntityId = expectedType switch
        {
            EIdentityType.User => (await _userRepository.GetByIdentityIdAsync(identityUser.Id))?.Id,
            EIdentityType.Workshop => (await _workshopRepository.GetByIdentityIdAsync(identityUser.Id))?.Id,
            _ => null
        };

        if (domainEntityId == Guid.Empty || domainEntityId is null)
        {
            return Error.Validation(EField.General, "Invalid email or password.");
        }

        var authenticatedUser = new AuthenticatedIdentity(
            identityUser.Id,
            (Guid)domainEntityId,
            identityUser.Email!,
            identityUser.Type
        );

        return authenticatedUser;
    }
}
