namespace eMechanic.Infrastructure.DAL.Transactions;

using eMechanic.Application.Abstractions.Identity;
using eMechanic.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

internal sealed class TransactionalExecutor : ITransactionalExecutor
{
    private readonly AppDbContext _appDbContext;
    private readonly IdentityAppDbContext _identityDbContext;

    public TransactionalExecutor(
        AppDbContext appDbContext,
        IdentityAppDbContext identityDbContext)
    {
        _appDbContext = appDbContext;
        _identityDbContext = identityDbContext;
    }

    public async Task ExecuteAsync(Func<Task> operation, CancellationToken ct = default)
    {
        var strategy = _appDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            _identityDbContext.Database.SetDbConnection(_appDbContext.Database.GetDbConnection());

            await using var transaction = await _appDbContext.Database.BeginTransactionAsync(ct);
            await _identityDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), ct);

            try
            {
                await operation();
                await transaction.CommitAsync(ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}
