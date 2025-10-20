namespace eMechanic.Common.CQRS;

using MediatR;
using Result;

public interface IResultQuery<TResult> : IRequest<Result<TResult, Error>>;
