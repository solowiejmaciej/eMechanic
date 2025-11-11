namespace eMechanic.Infrastructure.Services.Creators;

using System.Collections.ObjectModel;
using Application.Identity;
using Application.Users.Repositories;
using Application.Users.Services;
using Common.Result.Fields;
using Domain.User;
using eMechanic.Application.Abstractions.Identity;
using eMechanic.Common.Result;
using eMechanic.Infrastructure.Exceptions;
using eMechanic.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

internal sealed class UserService : IUserService
{
    private readonly UserManager<Identity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<Identity> userManager,
        IUserRepository userRepository,
        ITransactionalExecutor transactionalExecutor,
        ILogger<UserService> logger)
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

    public async Task<Result<Success, Error>> UpdateUserWithIdentityAsync(
        Guid domainUserId,
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        try
        {
            await _transactionalExecutor.ExecuteAsync(async () =>
            {
                var domainUser = await _userRepository.GetByIdAsync(domainUserId, cancellationToken);
                if (domainUser is null)
                {
                    throw new RegistrationException(new Error(EErrorCode.NotFoundError, "Domain user not found."));
                }

                var identityUser = await _userManager.FindByIdAsync(domainUser.IdentityId.ToString());
                if (identityUser is null)
                {
                    throw new RegistrationException(new Error(EErrorCode.InternalServerError, "Identity user not found."));
                }

                if (!string.Equals(identityUser.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingIdentity = await _userManager.FindByEmailAsync(email);
                    if (existingIdentity is not null && existingIdentity.Id != identityUser.Id)
                    {
                        throw new RegistrationException(Error.Validation(EField.Email, "Email already in use."));
                    }

                    var emailResult = await _userManager.SetEmailAsync(identityUser, email);
                    var userNameResult = await _userManager.SetUserNameAsync(identityUser, email);

                    if (!emailResult.Succeeded || !userNameResult.Succeeded)
                    {
                        var errors = emailResult.Errors.Concat(userNameResult.Errors).Select(e => e.Description).ToArray();
                        var errorDict = new ReadOnlyDictionary<string, string[]>(
                            new Dictionary<string, string[]> { { "IdentityErrors", errors } });
                        throw new RegistrationException(new Error(EErrorCode.ValidationError, errorDict));
                    }
                }

                domainUser.Update(email, firstName, lastName);

                _userRepository.UpdateAsync(domainUser, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

            }, cancellationToken);
        }
        catch (RegistrationException ex)
        {
            _logger.LogWarning(ex, "Failed to update user details for user {UserId} due to validation.", domainUserId);
            return ex.Error;
        }
        catch (ArgumentException ex)
        {
             _logger.LogWarning(ex, "Failed to update user details for user {UserId} due to invalid arguments.", domainUserId);
             return new Error(EErrorCode.ValidationError, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating user {UserId}.", domainUserId);
            return new Error(EErrorCode.InternalServerError);
        }

        return Result.Success;
    }
}
