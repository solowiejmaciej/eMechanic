namespace eMechanic.NotificationService;

using System.Reflection;
using eMechanic.NotificationService.DAL;
using eMechanic.NotificationService.Constans;
using Microsoft.EntityFrameworkCore;
using eMechanic.ServiceDefaults;
using Events;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();
        builder.Services.AddSwagger("eMechanic.NotificationService", WebApiConstans.CURRENT_API_VERSION);

        builder.Services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDb")));

        builder.Services.AddNotificationService(builder.Configuration);



        builder.Services.AddEventConsuming(builder.Configuration, Assembly.GetExecutingAssembly());


        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapDefaultEndpoints();

        var apiV1Group = app.MapGroup($"/api/{WebApiConstans.CURRENT_API_VERSION}");
        apiV1Group.MapFeatures();

        app.UseHttpsRedirection();
        app.Run();

    }
}
