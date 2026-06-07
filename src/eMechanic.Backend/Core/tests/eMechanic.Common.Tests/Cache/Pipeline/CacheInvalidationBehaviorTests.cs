namespace eMechanic.Common.Tests.Cache.Pipeline;

using eMechanic.Common.Cache.Abstractions;
using eMechanic.Common.Cache.Configuration;
using eMechanic.Common.Cache.Pipeline;
using eMechanic.Common.Result;
using Microsoft.Extensions.Logging;
using NSubstitute;

public sealed class CacheInvalidationBehaviorTests
{
    private readonly ICacheConfiguration _config = Substitute.For<ICacheConfiguration>();
    private readonly ICacheInvalidationService _invalidationService = Substitute.For<ICacheInvalidationService>();
    private readonly ILogger<CacheInvalidationBehavior<TestCommand, Result<int, Error>>> _logger =
        Substitute.For<ILogger<CacheInvalidationBehavior<TestCommand, Result<int, Error>>>>();

    private readonly CacheInvalidationBehavior<TestCommand, Result<int, Error>> _sut;

    public CacheInvalidationBehaviorTests()
    {
        _sut = new CacheInvalidationBehavior<TestCommand, Result<int, Error>>(
            _config,
            _invalidationService,
            _logger);
    }

    [Fact]
    public async Task Handle_Should_InvalidateCache_WhenCommandSucceeds()
    {
        var targets = new[] { typeof(TestQueryA), typeof(TestQueryB) };
        _config.TryGetInvalidationTargets(typeof(TestCommand), out Arg.Any<Type[]?>())
            .Returns(x =>
            {
                x[1] = targets;
                return true;
            });

        var next = Substitute.For<Func<CancellationToken, Task<Result<int, Error>>>>();
        Result<int, Error> success = 10;
        next.Invoke(Arg.Any<CancellationToken>()).Returns(success);

        var result = await _sut.Handle(new TestCommand(Guid.NewGuid()), next, CancellationToken.None);

        await _invalidationService.Received(1)
            .InvalidateAsync(typeof(TestCommand), targets, Arg.Any<CancellationToken>());
        Assert.False(result.HasError());
    }

    [Fact]
    public async Task Handle_Should_NotInvalidate_WhenCommandReturnsError()
    {
        var targets = new[] { typeof(TestQueryA) };
        _config.TryGetInvalidationTargets(typeof(TestCommand), out Arg.Any<Type[]?>())
            .Returns(x =>
            {
                x[1] = targets;
                return true;
            });

        var next = Substitute.For<Func<CancellationToken, Task<Result<int, Error>>>>();
        Result<int, Error> failure = new Error(EErrorCode.ValidationError, "invalid");
        next.Invoke(Arg.Any<CancellationToken>()).Returns(failure);

        var result = await _sut.Handle(new TestCommand(Guid.NewGuid()), next, CancellationToken.None);

        await _invalidationService.DidNotReceive()
            .InvalidateAsync(Arg.Any<Type>(), Arg.Any<IReadOnlyCollection<Type>>(), Arg.Any<CancellationToken>());
        Assert.True(result.HasError());
    }

    [Fact]
    public async Task Handle_Should_NotInvalidate_WhenNoTargetsConfigured()
    {
        _config.TryGetInvalidationTargets(typeof(TestCommand), out Arg.Any<Type[]?>()).Returns(false);

        var next = Substitute.For<Func<CancellationToken, Task<Result<int, Error>>>>();
        Result<int, Error> success = 3;
        next.Invoke(Arg.Any<CancellationToken>()).Returns(success);

        var result = await _sut.Handle(new TestCommand(Guid.NewGuid()), next, CancellationToken.None);

        await _invalidationService.DidNotReceive()
            .InvalidateAsync(Arg.Any<Type>(), Arg.Any<IReadOnlyCollection<Type>>(), Arg.Any<CancellationToken>());
        await next.Received(1).Invoke(Arg.Any<CancellationToken>());
        Assert.False(result.HasError());
    }

    public sealed record TestCommand(Guid Id);
    public sealed record TestQueryA(Guid Id);
    public sealed record TestQueryB(Guid Id);
}


