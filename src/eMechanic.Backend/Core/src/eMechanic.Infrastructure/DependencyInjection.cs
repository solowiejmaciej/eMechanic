namespace eMechanic.Infrastructure;

using Application.Abstractions.Identity;
using Application.Abstractions.User;
using Application.Abstractions.Workshop;
using DAL;
using DAL.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Services;
using Services.Creators;

public static class DependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>("eMechanic");
        builder.AddNpgsqlDbContext<IdentityAppDbContext>("eMechanic");
    }

    public static void AddInfrastructure(this IServiceCollection services)
    {
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
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserCreatorService, UserCreatorService>();
        services.AddScoped<IWorkshopCreatorService, WorkshopCreatorService>();
        services.AddScoped<ITransactionalExecutor, TransactionalExecutor>();
        services.AddScoped<IPaginationService, PaginationService>();
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
