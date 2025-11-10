namespace eMechanic.Application.Users.DomainEventsHandlers;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.UserRepairPreferences;
using eMechanic.Domain.User.DomainEvents;
using eMechanic.Domain.UserRepairPreferences;
using eMechanic.Domain.UserRepairPreferences.Enums;
using Microsoft.Extensions.Logging;

internal sealed class CreateUserRepairPreferencesUserCreatedDomainEventHandler
    : IDomainEventHandler<UserCreatedDomainEvent>
{
    private readonly IUserRepairPreferencesRepository _repairPreferencesRepository;
    private readonly ILogger<CreateUserRepairPreferencesUserCreatedDomainEventHandler> _logger;

    public CreateUserRepairPreferencesUserCreatedDomainEventHandler(
        IUserRepairPreferencesRepository repairPreferencesRepository,
        ILogger<CreateUserRepairPreferencesUserCreatedDomainEventHandler> logger)
    {
        _repairPreferencesRepository = repairPreferencesRepository;
        _logger = logger;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var preferences = UserRepairPreferences.Create(
                notification.User.Id,
                EPartsPreference.Balanced,
                ETimelinePreference.Standard
            );

            await _repairPreferencesRepository.AddAsync(preferences, cancellationToken);


            _logger.LogInformation("User repair preferences created for {UserId}", notification.User.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating user repair preferences for {UserId}", notification.User.Id);
        }
    }
}
