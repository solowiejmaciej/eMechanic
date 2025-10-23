namespace eMechanic.Application.Users.Register.UserCreated;

using Abstractions.DomainEvents;
using Domain.Users.DomainEvents;
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
