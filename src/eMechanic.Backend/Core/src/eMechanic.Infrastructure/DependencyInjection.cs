namespace eMechanic.Infrastructure;

using Application.Abstractions.Identity;
using Application.Abstractions.Identity.Contexts;
using Application.Abstractions.User;
using Application.Abstractions.UserRepairPreferences;
using Application.Abstractions.Vehicle;
using Application.Abstractions.VehicleTimeline;
using Application.Abstractions.Workshop;
using DAL;
using DAL.Transactions;
using Domain.Vehicle;
using Identity.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Services;
using Services.Creators;
using Services.Identity;

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
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserCreatorService, UserCreatorService>();
        services.AddScoped<IWorkshopCreatorService, WorkshopCreatorService>();
        services.AddScoped<ITransactionalExecutor, TransactionalExecutor>();
        services.AddScoped<IPaginationService, PaginationService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IAuthenticator, Authenticator>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IWorkshopContext, WorkshopContext>();
    }

    public static void ApplyMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();

        var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityAppDbContext>();
        identityDbContext.Database.Migrate();
    }
}
