namespace eMechanic.Infrastructure.Repositories.Specifications;

using System.Linq.Expressions;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; protected set; } = x => true;
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
}
