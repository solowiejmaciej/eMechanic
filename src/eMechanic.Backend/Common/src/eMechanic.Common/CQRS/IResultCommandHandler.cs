namespace eMechanic.Common.CQRS;

using MediatR;
using Result;

public interface IResultCommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse, Error>>
    where TCommand : IResultCommand<TResponse>;
