namespace eMechanic.Application.Users.DomainEventsHandlers;

using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Domain.User.DomainEvents;
using Microsoft.Extensions.Logging;

public class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;

    public UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UserCreatedDomainEvent received");

        return Task.CompletedTask;
    }
}
