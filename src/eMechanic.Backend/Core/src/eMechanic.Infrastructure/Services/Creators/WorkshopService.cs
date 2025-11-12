namespace eMechanic.Infrastructure.Services.Creators;

using System.Collections.ObjectModel;
using Application.Identity;
using Application.Workshop.Repositories;
using Application.Workshop.Services;
using Common.Result.Fields;
using eMechanic.Application.Abstractions.Identity;
using eMechanic.Common.Result;
using eMechanic.Domain.Workshop;
using eMechanic.Infrastructure.Exceptions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

internal sealed class WorkshopService : IWorkshopService
{
    private readonly UserManager<Identity> _userManager;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<WorkshopService> _logger;

    public WorkshopService(
        UserManager<Identity> userManager,
        IWorkshopRepository workshopRepository,
        ITransactionalExecutor transactionalExecutor,
        ILogger<WorkshopService> logger)
    {
        _userManager = userManager;
        _workshopRepository = workshopRepository;
        _transactionalExecutor = transactionalExecutor;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> CreateWorkshopWithIdentityAsync(
        string email, string password, string contactEmail, string name,
        string displayName, string phoneNumber, string address, string city,
        string postalCode, string country, CancellationToken cancellationToken)
    {
        var identityExists = await _userManager.FindByEmailAsync(email);
        if (identityExists is not null)
        {
            return Error.Validation(EField.Email, "Identity with given email already exists.");
        }

        var identity = Identity.Create(email, EIdentityType.Workshop);

        Guid domainWorkshopId = Guid.Empty;

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

                var domainWorkshop = Workshop.Create(
                    contactEmail, email, displayName, name,
                    phoneNumber, address, city, postalCode, country,
                    identity.Id
                );

                await _workshopRepository.AddAsync(domainWorkshop, cancellationToken);
                await _workshopRepository.SaveChangesAsync(cancellationToken);

                domainWorkshopId = domainWorkshop.Id;

                if (domainWorkshopId == Guid.Empty)
                {
                    throw new RegistrationException(new Error(EErrorCode.InternalServerError, "Failed to create domain workshop."));
                }

            }, cancellationToken);
        }
        catch (RegistrationException ex)
        {
            return ex.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering a new workshop with email {Email}.", email);
            return new Error(EErrorCode.InternalServerError);
        }

        return domainWorkshopId;
    }

    public async Task<Result<Success, Error>> UpdateWorkshopWithIdentityAsync(
        Guid domainWorkshopId,
        string email,
        string contactEmail,
        string name,
        string displayName,
        string phoneNumber,
        string address,
        string city,
        string postalCode,
        string country,
        CancellationToken cancellationToken)
    {
        try
        {
            await _transactionalExecutor.ExecuteAsync(async () =>
            {
                var domainWorkshop = await _workshopRepository.GetByIdAsync(domainWorkshopId, cancellationToken);
                if (domainWorkshop is null)
                {
                    throw new RegistrationException(new Error(EErrorCode.NotFoundError, "Domain workshop not found."));
                }

                var identityUser = await _userManager.FindByIdAsync(domainWorkshop.IdentityId.ToString());
                if (identityUser is null)
                {
                    throw new RegistrationException(new Error(EErrorCode.InternalServerError, "Identity user not found."));
                }

                if (!string.Equals(identityUser.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingIdentity = await _userManager.FindByEmailAsync(email);
                    if (existingIdentity is not null && existingIdentity.Id != identityUser.Id)
                    {
                        throw new RegistrationException(Error.Validation(EField.Email, "Email (login) already in use."));
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

                domainWorkshop.Update(
                    email,
                    contactEmail,
                    name,
                    displayName,
                    phoneNumber,
                    address,
                    city,
                    postalCode,
                    country
                );

                _workshopRepository.UpdateAsync(domainWorkshop, cancellationToken);
                await _workshopRepository.SaveChangesAsync(cancellationToken);

            }, cancellationToken);
        }
        catch (RegistrationException ex)
        {
            _logger.LogWarning(ex, "Failed to update workshop details for {WorkshopId} due to validation.", domainWorkshopId);
            return ex.Error;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Failed to update workshop details for {WorkshopId} due to invalid arguments.", domainWorkshopId);
            return new Error(EErrorCode.ValidationError, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating workshop {WorkshopId}.", domainWorkshopId);
            return new Error(EErrorCode.InternalServerError);
        }

        return Result.Success;
    }
}
