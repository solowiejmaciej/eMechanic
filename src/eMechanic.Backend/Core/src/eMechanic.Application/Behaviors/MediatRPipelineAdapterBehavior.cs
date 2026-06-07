namespace eMechanic.Application.Behaviors;

using eMechanic.Common.CQRS;
using MediatR;

/// <summary>
/// Bridges custom pipeline abstractions (<see cref="IResultPipelineBehavior{TRequest,TResponse}"/>)
/// to MediatR's <see cref="IPipelineBehavior{TRequest,TResponse}"/>.
/// This keeps feature behaviors decoupled from direct MediatR contracts.
/// </summary>
public sealed class MediatRPipelineAdapterBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IResultPipelineBehavior<TRequest, TResponse>> _behaviors;

    public MediatRPipelineAdapterBehavior(IEnumerable<IResultPipelineBehavior<TRequest, TResponse>> behaviors)
    {
        _behaviors = behaviors;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Func<CancellationToken, Task<TResponse>> chain = ct => next(ct);

        foreach (var behavior in _behaviors.Reverse())
        {
            var currentNext = chain;
            chain = ct => behavior.Handle(request, currentNext, ct);
        }

        return chain(cancellationToken);
    }
}





