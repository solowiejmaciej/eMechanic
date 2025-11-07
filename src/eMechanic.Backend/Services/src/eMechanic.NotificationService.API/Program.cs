namespace eMechanic.NotificationService;

using System.Reflection;
using Common.Web;
using Constans;
using Events;
using ServiceDefaults;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();
        builder.Services.AddSwagger("eMechanic.NotificationService", WebApiConstans.CURRENT_API_VERSION);
        builder.Services.AddEventConsuming(builder.Configuration, Assembly.GetExecutingAssembly());

        var app = builder.Build();

        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapDefaultEndpoints();

        var apiV1Group = app.MapGroup($"/api/{WebApiConstans.CURRENT_API_VERSION}");
        apiV1Group.MapFeatures();

        app.UseHttpsRedirection();
        app.Run();
    }
}
