namespace eMechanic.Application.Abstractions.Identity;

public interface ITransactionalExecutor
{
    Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken);
}
