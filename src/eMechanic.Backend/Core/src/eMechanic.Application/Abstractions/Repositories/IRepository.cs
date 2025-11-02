namespace eMechanic.Application.Abstractions.Repositories;

using Common.DDD;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> AddAsync(T entity, CancellationToken cancellationToken);
    void UpdateAsync(T entity, CancellationToken cancellationToken);
    void DeleteAsync(T entity, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
