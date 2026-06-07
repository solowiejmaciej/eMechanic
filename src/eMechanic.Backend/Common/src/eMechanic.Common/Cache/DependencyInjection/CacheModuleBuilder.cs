namespace eMechanic.Common.Cache.DependencyInjection;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public sealed class CacheModuleBuilder
{
    internal Assembly RequestsAssembly { get; }

    internal ServiceLifetime KeyGeneratorLifetime { get; private set; } = ServiceLifetime.Scoped;
    internal ServiceLifetime InvalidationServiceLifetime { get; private set; } = ServiceLifetime.Scoped;

    internal bool RegisterValidationBehavior { get; private set; } = true;
    internal bool RegisterCachingBehavior { get; private set; } = true;
    internal bool RegisterInvalidationBehavior { get; private set; } = true;

    internal CacheModuleBuilder(Assembly requestsAssembly)
    {
        RequestsAssembly = requestsAssembly;
    }

    public CacheModuleBuilder WithoutValidationBehavior()
    {
        RegisterValidationBehavior = false;
        return this;
    }

    public CacheModuleBuilder WithoutCachingBehavior()
    {
        RegisterCachingBehavior = false;
        return this;
    }

    public CacheModuleBuilder WithoutInvalidationBehavior()
    {
        RegisterInvalidationBehavior = false;
        return this;
    }

    public CacheModuleBuilder WithKeyGeneratorLifetime(ServiceLifetime lifetime)
    {
        KeyGeneratorLifetime = lifetime;
        return this;
    }

    public CacheModuleBuilder WithInvalidationServiceLifetime(ServiceLifetime lifetime)
    {
        InvalidationServiceLifetime = lifetime;
        return this;
    }
}

