
namespace eMechanic.Integration.Tests.TestContainers;

using API;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.TestHost;

using eMechanic.Infrastructure.DAL;

using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public IntegrationTestWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("test_emechanic")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(5432))
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var appDbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (appDbContextDescriptor != null)
            {
                services.Remove(appDbContextDescriptor);
            }
            var identityDbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IdentityAppDbContext>));
            if (identityDbContextDescriptor != null)
            {
                services.Remove(identityDbContextDescriptor);
            }
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IdentityAppDbContext>();


            var connectionString = _dbContainer.GetConnectionString();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseNpgsql(connectionString));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

         using var scope = Services.CreateScope();
         var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
         await appDbContext.Database.MigrateAsync();

         var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityAppDbContext>();
         await identityDbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
