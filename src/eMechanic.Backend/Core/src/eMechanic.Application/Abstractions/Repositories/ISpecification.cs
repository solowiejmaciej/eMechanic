namespace eMechanic.Infrastructure.Repositories.Specifications;

using System.Linq.Expressions;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
}
