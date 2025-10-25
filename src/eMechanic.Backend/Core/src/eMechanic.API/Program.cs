using eMechanic.API.Constans;
using eMechanic.API.Middleware;

namespace eMechanic.API;

using Application;
using Features;
using Infrastructure;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.AddInfrastructure();
        builder.Services.AddOpenApi();
        builder.Services.AddSwagger("eMechanic API", WebApiConstans.CURRENT_API_VERSION);
        builder.AddApi();
        builder.Services.AddApi(builder.Configuration);

        var app = builder.Build();

        app.MapDefaultEndpoints();

        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.AddApi();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();
        var apiV1Group = app.MapGroup($"/api/{WebApiConstans.CURRENT_API_VERSION}");
        apiV1Group.MapFeatures();

        app.Services.ApplyMigrations();

        app.Run();
    }
}
