namespace eMechanic.Common.CQRS;

using MediatR;
using Result;

public interface IResultCommand<TResponse> : IRequest<Result<TResponse, Error>>;
