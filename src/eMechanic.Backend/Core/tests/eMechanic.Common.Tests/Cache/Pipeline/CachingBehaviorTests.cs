namespace eMechanic.Common.Tests.Cache.Pipeline;

using System.Text;
using System.Text.Json;
using eMechanic.Common.Cache.Abstractions;
using eMechanic.Common.Cache.Configuration;
using eMechanic.Common.Cache.Pipeline;
using eMechanic.Common.Result;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;

public sealed class CachingBehaviorTests
{
    private readonly IDistributedCache _cache = Substitute.For<IDistributedCache>();
    private readonly ILogger<CachingBehavior<TestRequest, Result<int, Error>>> _logger =
        Substitute.For<ILogger<CachingBehavior<TestRequest, Result<int, Error>>>>();
    private readonly ICacheConfiguration _config = Substitute.For<ICacheConfiguration>();
    private readonly ICacheKeyGenerator _keyGenerator = Substitute.For<ICacheKeyGenerator>();

    private readonly CachingBehavior<TestRequest, Result<int, Error>> _sut;

    public CachingBehaviorTests()
    {
        _sut = new CachingBehavior<TestRequest, Result<int, Error>>(_cache, _logger, _config, _keyGenerator);

        object boxedRule = new CacheRule<TestRequest>(TimeSpan.FromMinutes(5), ECacheScope.Public);
        _config.TryGetRule(typeof(TestRequest), out Arg.Any<object?>())
            .Returns(x =>
            {
                x[1] = boxedRule;
                return true;
            });

        _keyGenerator.GenerateKey(Arg.Any<CacheRule<TestRequest>>(), Arg.Any<TestRequest>()).Returns("cache-key");
        _cache.GetAsync("cacheGroup:TestRequest", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);
    }

    [Fact]
    public async Task Handle_Should_ReturnCachedResponse_AndSkipNext()
    {
        var request = new TestRequest(Guid.NewGuid());
        Result<int, Error> cached = 123;
        var cachedJson = JsonSerializer.Serialize(cached);

        _cache.GetAsync("cache-key:0", Arg.Any<CancellationToken>())
            .Returns(Encoding.UTF8.GetBytes(cachedJson));

        var next = Substitute.For<Func<CancellationToken, Task<Result<int, Error>>>>();

        var result = await _sut.Handle(request, next, CancellationToken.None);

        await next.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
        Assert.False(result.HasError());
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public async Task Handle_Should_WriteToCache_WhenNextReturnsSuccess()
    {
        var request = new TestRequest(Guid.NewGuid());
        _cache.GetAsync("cache-key:0", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);

        var next = Substitute.For<Func<CancellationToken, Task<Result<int, Error>>>>();
        Result<int, Error> success = 77;
        next.Invoke(Arg.Any<CancellationToken>()).Returns(success);

        var result = await _sut.Handle(request, next, CancellationToken.None);

        await next.Received(1).Invoke(Arg.Any<CancellationToken>());
        await _cache.Received(1).SetAsync(
            "cache-key:0",
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
        Assert.Equal(77, result.Value);
    }

    [Fact]
    public async Task Handle_Should_NotWriteToCache_WhenNextReturnsError()
    {
        var request = new TestRequest(Guid.NewGuid());
        _cache.GetAsync("cache-key:0", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);

        var next = Substitute.For<Func<CancellationToken, Task<Result<int, Error>>>>();
        Result<int, Error> failure = new Error(EErrorCode.ValidationError, "validation");
        next.Invoke(Arg.Any<CancellationToken>()).Returns(failure);

        var result = await _sut.Handle(request, next, CancellationToken.None);

        await _cache.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
        Assert.True(result.HasError());
    }

    public sealed record TestRequest(Guid Id);
}

