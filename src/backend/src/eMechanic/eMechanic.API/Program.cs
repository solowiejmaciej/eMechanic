using eMechanic.API.Features;
using eMechanic.API.Middleware;
using eMechanic.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapFeatures();

app.Run();
