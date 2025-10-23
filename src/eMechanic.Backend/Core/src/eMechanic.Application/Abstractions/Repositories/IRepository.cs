namespace eMechanic.Application.Abstractions.Repositories;

using Common.DDD;
using Common.Result;
using Infrastructure.Repositories.Specifications;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> AddAsync(T entity, CancellationToken cancellationToken);
    void UpdateAsync(T entity, CancellationToken cancellationToken);
    void DeleteAsync(T entity, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<PaginationResult<T>> GetAllPaginatedAsync(
        ISpecification<T> specification,
        IPaginationParameters paginationParameters,
        CancellationToken cancellationToken);

    Task<PaginationResult<T>> GetAllPaginatedAsync(
        IEnumerable<ISpecification<T>> specifications,
        IPaginationParameters paginationParameters,
        CancellationToken cancellationToken);

    Task<T?> GetFirstOrDefaultAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken);

    Task<T?> GetSingleAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken);

    Task<T?> GetSingleAsync(
        IEnumerable<ISpecification<T>> specifications,
        CancellationToken cancellationToken);
}
