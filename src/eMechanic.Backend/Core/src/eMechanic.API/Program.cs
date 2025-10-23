using eMechanic.API.Constans;
using eMechanic.API.Features;
using eMechanic.API.Middleware;
using eMechanic.Application;
using eMechanic.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
var apiV1Group = app.MapGroup($"/api/{WebApiConstans.CURRENT_API_VERSION}");
apiV1Group.MapFeatures();

app.Services.ApplyMigrations();

app.Run();
