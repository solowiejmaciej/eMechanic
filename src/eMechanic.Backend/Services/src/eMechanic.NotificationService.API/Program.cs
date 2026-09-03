using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using eMechanic.NotificationService.DAL;
using eMechanic.NotificationService.Services.Abstractions;
using eMechanic.NotificationService.Services;
using eMechanic.NotificationService.Services.Infrastructure;
using System;
using eMechanic.NotificationService;

var builder = Host.CreateDefaultBuilder(args);


builder.ConfigureServices((hostContext, services) =>
{
    var configuration = hostContext.Configuration;
    services.AddNotificationService(configuration);
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<eMechanic.NotificationService.DAL.NotificationDbContext>();

    await dbContext.Database.MigrateAsync();}

Console.WriteLine("NotificationService Worker został uruchomiony. Nasłuchiwanie na zdarzenia");
await host.RunAsync();
