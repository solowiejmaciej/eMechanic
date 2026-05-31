namespace eMechanic.Integration.Tests.Payment;

using Application.Abstractions.Storage;
using Application.Payments.Abstractions;
using Application.Summary;
using DotNet.Testcontainers.Builders;
using eMechanic.Infrastructure.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mocks;
using NSubstitute;
using Testcontainers.PostgreSql;

public sealed class PaymentIntegrationTestWebAppFactory
    : WebApplicationFactory<eMechanic.API.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public MockPaymentWebhookProcessor MockWebhookProcessor { get; } = new();

    public PaymentIntegrationTestWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("test_emechanic_payment")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(5432))
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var appDbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (appDbContextDescriptor != null)
            {
                services.Remove(appDbContextDescriptor);
            }

            var identityDbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<IdentityAppDbContext>));
            if (identityDbContextDescriptor != null)
            {
                services.Remove(identityDbContextDescriptor);
            }

            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IdentityAppDbContext>();
            services.RemoveAll<IModelFacade>();
            services.RemoveAll<IFileStorageService>();
            services.RemoveAll<IPaymentWebhookProcessor>();
            services.RemoveAll<IPaymentService>();

            var mockFacade = Substitute.For<IModelFacade>();
            mockFacade.GetResponseAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult("AI Generated Summary Test Content"));
            services.AddScoped(_ => mockFacade);

            var mockPaymentService = Substitute.For<IPaymentService>();
            services.AddScoped(_ => mockPaymentService);

            services.AddSingleton<IPaymentWebhookProcessor>(MockWebhookProcessor);

            var connectionString = _dbContainer.GetConnectionString();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddSingleton<IFileStorageService, MockFileStorageService>();
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

