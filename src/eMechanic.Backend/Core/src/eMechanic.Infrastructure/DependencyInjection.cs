namespace eMechanic.Infrastructure;

using Application.Abstractions.Identity;
using Application.Abstractions.Identity.Contexts;
using Application.Abstractions.Outbox;
using Application.Abstractions.Storage;
using Application.Summary;
using Application.UserRepairPreferences.Repositories;
using Application.Users.Repositories;
using Application.Users.Services;
using Application.Vehicle.Document.Repositories;
using Application.Vehicle.Vehicle.Repositories;
using Application.Workshop.Document.Repositories;
using Application.Workshop.Workshop.Repositories;
using Application.Workshop.Workshop.Services;
using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Payments.Repositories;
using eMechanic.Application.Repair.Repositories;
using eMechanic.Application.RepairRequest.Repositories;
using DAL;
using DAL.Transactions;
using Domain.Vehicle;
using Identity.Contexts;
using LLM.Builders;
using LLM.Enums;
using LLM.Factories;
using LLM.Models;
using LLM.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Outbox;
using Payments;
using Repositories;
using Services;
using Services.Creators;
using Services.Identity;
using Services.Identity.ExternalProviders.Google;
using Storage;
using Storage.Builders;

public static class DependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.EnrichNpgsqlDbContext<AppDbContext>();
        builder.EnrichNpgsqlDbContext<IdentityAppDbContext>();
    }

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("eMechanic")));

        services.AddDbContext<IdentityAppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("eMechanic")));

        services.AddIdentity<Identity.Identity, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(configuration.GetConnectionString("Storage"));
        });

        services.AddOptions<StripeOptions>().BindConfiguration("Stripe");

        services.AddRepositories();
        services.AddServices();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkshopRepository, WorkshopRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IVehicleTimelineRepository, VehicleTimelineRepository>();
        services.AddScoped<IUserRepairPreferencesRepository, UserRepairPreferencesRepositoryRepository>();
        services.AddScoped<IVehicleDocumentRepository, VehicleDocumentRepository>();
        services.AddScoped<IWorkshopDocumentRepository, WorkshopDocumentRepository>();
        services.AddScoped<IRepairRequestRepository, RepairRequestRepository>();
        services.AddScoped<IRepairRepository, RepairRepository>();
        services.AddScoped<IPaymentOrderRepository, PaymentOrderRepository>();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkshopService, WorkshopService>();
        services.AddScoped<ITransactionalExecutor, TransactionalExecutor>();
        services.AddScoped<IPaginationService, PaginationService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IAuthenticator, Authenticator>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IWorkshopContext, WorkshopContext>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IFileStorageService, AzureBlobStorageService>();
        services.AddScoped<IVehicleDocumentPathBuilder, VehicleDocumentPathBuilder>();
        services.AddScoped<IWorkshopDocumentPathBuilder, WorkshopDocumentPathBuilder>();
        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
        services.AddScoped<IStripePaymentService, StripePaymentService>();
        services.AddScoped<IPaymentProcessor, StripePaymentProcessor>();
        services.RegisterLlmServices();
    }

    public static void ApplyMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();

        var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityAppDbContext>();
        identityDbContext.Database.Migrate();
    }

    private static void RegisterLlmServices(this IServiceCollection services)
    {
        services.AddSingleton<IModelFactory, ModelFactory>();
        services.AddSingleton<ChatRequestBuilder>();
        services.AddScoped<IModel>(provider =>
        {
            var factory = provider.GetRequiredService<IModelFactory>();
            var modelType = ModelProviderType.Google;
            return factory.GetClient(modelType);
        });
        services.AddScoped<IModelFacade, ModelFacade>();
    }
}
