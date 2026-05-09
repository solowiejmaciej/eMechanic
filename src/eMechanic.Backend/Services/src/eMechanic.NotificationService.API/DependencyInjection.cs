namespace eMechanic.NotificationService;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using eMechanic.NotificationService.Services;
using eMechanic.NotificationService.Services.Infrastructure;
using eMechanic.NotificationService.Services.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<NotificationSettings>(configuration.GetSection(NotificationSettings.SECTION_NAME));

        services.AddHttpClient<IEmailService, EmailLabsApiService>();

        services.AddScoped<ISmsService, TwilioSmsService>();

        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

        return services;
    }

    public static void AddSwagger(this IServiceCollection services, string title, string version)
    {
        services.AddSwaggerGen();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        });
    }
}
