namespace eMechanic.Infrastructure.Services.Creators;

using System.Collections.ObjectModel;
using Common.Result.Fields;
using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Abstractions.User;
using eMechanic.Common.Result;
using eMechanic.Domain.Users;
using eMechanic.Infrastructure.Exceptions;
using eMechanic.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

internal sealed class UserCreatorService : IUserCreatorService
{
    private readonly UserManager<Identity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<UserCreatorService> _logger;

    public UserCreatorService(
        UserManager<Identity> userManager,
        IUserRepository userRepository,
        ITransactionalExecutor transactionalExecutor,
        ILogger<UserCreatorService> logger)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _transactionalExecutor = transactionalExecutor;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> CreateUserWithIdentityAsync(
        string email, string password, string firstName, string lastName, CancellationToken cancellationToken)
    {
        var identityExists = await _userManager.FindByEmailAsync(email);
        if (identityExists is not null)
        {
            return Error.Validation(EField.Email, "Identity with given email already exists.");
        }

        var identity = Identity.Create(email, EIdentityType.User);

        Guid domainUserId = Guid.Empty;

        try
        {
            await _transactionalExecutor.ExecuteAsync(async () =>
            {
                var identityResult = await _userManager.CreateAsync(identity, password);

                if (!identityResult.Succeeded)
                {
                    var errors = identityResult.Errors.Select(e => e.Description).ToArray();
                    var errorDict = new ReadOnlyDictionary<string, string[]>(
                        new Dictionary<string, string[]> { { "IdentityErrors", errors } });
                    throw new RegistrationException(new Error(EErrorCode.ValidationError, errorDict));
                }

                var domainUser = User.Create(email, firstName, lastName, identity.Id);

                await _userRepository.AddAsync(domainUser, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                domainUserId = domainUser.Id;

                if(domainUserId == Guid.Empty)
                {
                    throw new RegistrationException(new Error(EErrorCode.InternalServerError, "Failed to create domain user."));
                }

            }, cancellationToken);
        }
        catch (RegistrationException ex)
        {
            return ex.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering a new user with email {Email}.", email);
            return new Error(EErrorCode.InternalServerError);
        }

        return domainUserId;
    }
}
