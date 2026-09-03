namespace eMechanic.NotificationService;

using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using eMechanic.Events;
using eMechanic.NotificationService.DAL;
using eMechanic.NotificationService.Services;
using eMechanic.NotificationService.Services.Infrastructure;
using eMechanic.NotificationService.Services.Abstractions;

public static class DependecyInjection
{
    public static IServiceCollection AddNotificationService(this IServiceCollection services,
        IConfiguration configuration)
    {
        var baseConnectionString = configuration.GetConnectionString("eMechanic");
        string notificationDbConnectionString;

        if (!string.IsNullOrWhiteSpace(baseConnectionString))
        {
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(baseConnectionString);
            builder.Database = "eMechanic_Notifications";
            notificationDbConnectionString = builder.ConnectionString;

        }
        else
        {
            notificationDbConnectionString = "Host=localhost;Port=5432;Database=eMechanic_Notifications;Username=postgres;Password=HCn{QDGTU*4e1e~H4hnVHu";
        }

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(notificationDbConnectionString));

        services.Configure<NotificationSettings>(configuration.GetSection(NotificationSettings.SECTION_NAME));

        services.AddHttpClient<IEmailService, EmailLabsApiService>();
        services.AddScoped<ISmsService, TwilioSmsService>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

        services.AddEventConsuming(configuration, Assembly.GetExecutingAssembly());

        return services;
    }
}
