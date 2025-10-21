namespace eMechanic.Application.Behaviors;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Result;
using FluentValidation;
using MediatR;

public sealed class QueryValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public QueryValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var responseType = typeof(TResponse);
        if (!responseType.IsGenericType ||
            responseType.GetGenericTypeDefinition() != typeof(Result<,>) ||
            responseType.GetGenericArguments()[1] != typeof(Error))
        {
            return await next(cancellationToken);
        }

        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        var validationErrors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(x => x.Key, x => x.Distinct().ToArray());

        var error = new Error(EErrorCode.ValidationError, validationErrors);

        return ConvertErrorToResult<TResponse>(error);
    }

    private static readonly ConcurrentDictionary<Type, Delegate> _converterCache = new();

    private static TResult ConvertErrorToResult<TResult>(Error error)
    {
        var resultType = typeof(TResult);

        var converter = _converterCache.GetOrAdd(resultType, type =>
        {
            var implicitMethod = type.GetMethod(
                "op_Implicit",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Error) },
                null
            );

            if (implicitMethod == null)
            {
                throw new InvalidOperationException(
                    $"Implicit operator from Error to {type.Name} not found");
            }

            var errorParam = Expression.Parameter(typeof(Error), "error");
            var call = Expression.Call(implicitMethod, errorParam);
            var lambda = Expression.Lambda<Func<Error, TResult>>(call, errorParam);

            return lambda.Compile();
        });

        return ((Func<Error, TResult>)converter)(error);
    }
}


// public sealed class QueryValidationBehavior<TRequest, TResponse>
//     : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IRequest<TResponse>
// {
//     private readonly IEnumerable<IValidator<TRequest>> _validators;
//
//     public QueryValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
//     {
//         _validators = validators;
//     }
//
//     public async Task<TResponse> Handle(
//         TRequest request,
//         RequestHandlerDelegate<TResponse> next,
//         CancellationToken cancellationToken)
//     {
//         if (!_validators.Any())
//         {
//             return await next(cancellationToken);
//         }
//
//         var context = new ValidationContext<TRequest>(request);
//
//         var validationResults = await Task.WhenAll(
//             _validators.Select(v => v.ValidateAsync(context, cancellationToken))
//         );
//
//         var failures = validationResults
//             .SelectMany(r => r.Errors)
//             .Where(f => f != null)
//             .ToList();
//
//         if (failures.Count == 0)
//         {
//             return await next(cancellationToken);
//         }
//
//         var validationErrors = failures
//             .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
//             .ToDictionary(x => x.Key, x => x.Distinct().ToArray());
//
//         var error = new Error(EErrorCode.ValidationError, validationErrors);
//
//         dynamic errorDynamic = error;
//         return errorDynamic;
//     }
// }
