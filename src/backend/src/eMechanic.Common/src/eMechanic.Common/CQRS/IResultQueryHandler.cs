namespace eMechanic.Common.CQRS;

using MediatR;
using Result;

public interface IResultQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, Result<TResult, Error>>
    where TQuery : IResultQuery<TResult>;
