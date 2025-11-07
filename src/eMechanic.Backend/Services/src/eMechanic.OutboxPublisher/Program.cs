using eMechanic.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var dbConnectionString = builder.Configuration.GetConnectionString("eMechanic");

if (string.IsNullOrEmpty(dbConnectionString))
{
    throw new InvalidOperationException("Connection string 'eMechanic' not found.");
}

builder.Services.AddNpgsqlDataSource(dbConnectionString);
builder.Services.AddEventPublishing(builder.Configuration);

builder.Build().Run();
