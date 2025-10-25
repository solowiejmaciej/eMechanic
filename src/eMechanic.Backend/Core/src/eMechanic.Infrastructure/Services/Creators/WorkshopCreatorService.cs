namespace eMechanic.Infrastructure.Services.Creators;

using System.Collections.ObjectModel;
using Common.Result.Fields;
using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Abstractions.Workshop;
using eMechanic.Common.Result;
using eMechanic.Domain.Workshop;
using eMechanic.Infrastructure.Exceptions;
using Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

internal sealed class WorkshopCreatorService : IWorkshopCreatorService
{
    private readonly UserManager<Identity> _userManager;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<WorkshopCreatorService> _logger;

    public WorkshopCreatorService(
        UserManager<Identity> userManager,
        IWorkshopRepository workshopRepository,
        ITransactionalExecutor transactionalExecutor,
        ILogger<WorkshopCreatorService> logger)
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
}
