namespace eMechanic.Common.Cache.DependencyInjection;

using System.Reflection;
using Abstractions;
using Attributes;
using Configuration;
using eMechanic.Common.CQRS;
using eMechanic.Common.CQRS.Pipeline;
using eMechanic.Common.Cache.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

public static class CacheDependencyInjection
{
    public static void AddCache(this WebApplicationBuilder builder, string connectionName)
    {
        var redisConnectionString = builder.Configuration.GetConnectionString(connectionName);

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            builder.AddRedisDistributedCache(connectionName);
            return;
        }

        builder.Services.AddDistributedMemoryCache();
    }

    public static IServiceCollection AddCache(this IServiceCollection services, Assembly requestsAssembly) => services.AddCache(requestsAssembly, _ => { });

    private static IServiceCollection AddCache(
        this IServiceCollection services,
        Assembly requestsAssembly,
        Action<CacheModuleBuilder> configure)
    {
        var builder = new CacheModuleBuilder(requestsAssembly);
        configure(builder);

        if (builder.RegisterValidationBehavior)
        {
            services.AddTransient(typeof(IResultPipelineBehavior<,>), typeof(QueryValidationBehavior<,>));
        }

        if (builder.RegisterCachingBehavior)
        {
            services.AddTransient(typeof(IResultPipelineBehavior<,>), typeof(CachingBehavior<,>));
        }

        if (builder.RegisterInvalidationBehavior)
        {
            services.AddTransient(typeof(IResultPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
        }

        services.AddCacheConfiguration(builder);
        return services;
    }

    private static void AddCacheConfiguration(this IServiceCollection services, CacheModuleBuilder builder)
    {
        services.Add(new ServiceDescriptor(typeof(ICacheKeyGenerator), typeof(CacheKeyGenerator), builder.KeyGeneratorLifetime));
        services.Add(new ServiceDescriptor(typeof(ICacheInvalidationService), typeof(CacheInvalidationService), builder.InvalidationServiceLifetime));

        var cacheConfig = new CacheConfiguration();

        var typesWithCache = builder.RequestsAssembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(CacheAttribute), false).Length != 0);

        foreach (var type in typesWithCache)
        {
            var attr = (CacheAttribute)type.GetCustomAttributes(typeof(CacheAttribute), false).First();
            var ruleType = typeof(CacheRule<>).MakeGenericType(type);
            var rule = Activator.CreateInstance(ruleType, new object[] { TimeSpan.FromSeconds(attr.DurationSeconds), attr.Scope });
            var registerMethod = typeof(CacheConfiguration).GetMethod(nameof(CacheConfiguration.Register))!.MakeGenericMethod(type);
            registerMethod.Invoke(cacheConfig, [rule!]);
        }

        var typesWithInvalidation = builder.RequestsAssembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(InvalidatesCacheAttribute), false).Length != 0);

        foreach (var type in typesWithInvalidation)
        {
            var attr = (InvalidatesCacheAttribute)type.GetCustomAttributes(typeof(InvalidatesCacheAttribute), false).First();
            cacheConfig.RegisterInvalidation(type, attr.QueryTypes);
        }

        services.AddSingleton<ICacheConfiguration>(cacheConfig);
    }
}



