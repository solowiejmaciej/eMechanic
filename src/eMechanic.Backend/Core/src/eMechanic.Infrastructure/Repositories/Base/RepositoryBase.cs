namespace eMechanic.Infrastructure.Repositories.Base;

using Application.Abstractions.Repositories;
using Common.DDD;
using Common.Result;
using DAL;
using Microsoft.EntityFrameworkCore;
using Services;
using Specifications;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly IPaginationService _paginationService;

    public Repository(AppDbContext context, IPaginationService paginationService)
    {
        _context = context;
        _paginationService = paginationService;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => await _dbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PaginationResult<T>> GetAllPaginatedAsync(
        ISpecification<T> specification,
        IPaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var specificationsQuery = ApplySpecification(specification);
        return await _paginationService.GetPaginatedResultAsync(specificationsQuery, paginationParameters,
            cancellationToken);
    }

    public async Task<PaginationResult<T>> GetAllPaginatedAsync(
        IEnumerable<ISpecification<T>> specifications,
        IPaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var specificationsQuery = ApplySpecifications(specifications);
        return await _paginationService.GetPaginatedResultAsync(specificationsQuery, paginationParameters,
            cancellationToken);
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken) =>
        await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public async Task<T?> GetSingleAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken) =>
        await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);

    public async Task<T?> GetSingleAsync(
        IEnumerable<ISpecification<T>> specifications,
        CancellationToken cancellationToken) =>
        await ApplySpecifications(specifications).SingleOrDefaultAsync(cancellationToken);

    public async Task<Guid> AddAsync(T entity, CancellationToken cancellationToken)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity.Id;
    }

    public void UpdateAsync(T entity, CancellationToken cancellationToken) => _context.Entry(entity).CurrentValues.SetValues(entity);

    public void DeleteAsync(T entity, CancellationToken cancellationToken) => _dbSet.Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var entry in _context.ChangeTracker.Entries<Entity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("A concurrency conflict occurred while saving changes", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("An error occurred while saving changes to the database", ex);
        }
    }

    private IQueryable<T> ApplySpecification(ISpecification<T>? specification)
    {
        var query = _dbSet.AsQueryable();

        if (specification == null) return query;

        query = query.Where(specification.Criteria);

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }

    private IQueryable<T> ApplySpecifications(IEnumerable<ISpecification<T>> specifications)
    {
        var query = _dbSet.AsQueryable();

        foreach (var specification in specifications)
        {
            query = query.Where(specification.Criteria);

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return query;
    }
}
