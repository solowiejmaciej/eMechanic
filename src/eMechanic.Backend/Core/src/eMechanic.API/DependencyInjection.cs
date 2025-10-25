namespace eMechanic.API;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApi(this WebApplicationBuilder builder) => builder.AddRedisDistributedCache("emechanic-cache");

    public static void AddApi(this IServiceCollection services)
    {
    }
}
