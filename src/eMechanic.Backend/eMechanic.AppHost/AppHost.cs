var builder = DistributedApplication.CreateBuilder(args);

var postgresServer = builder
    .AddPostgres("emechanic-postgres-server")
    .WithContainerName("emechanic-postgres")
    .WithDataVolume()
    .WithHostPort(5433)
    .WithLifetime(ContainerLifetime.Persistent);

var postgresDb = postgresServer.AddDatabase("eMechanic");
var redisCache = builder.AddRedis("emechanic-cache");
var serviceBus = builder.AddConnectionString("AzureServiceBus");

builder
    .AddProject<Projects.eMechanic_API>("eMechanic-Core")
    .WithReference(postgresDb)
    .WithReference(redisCache)
    .WaitFor(postgresServer);

builder.AddAzureFunctionsProject<Projects.eMechanic_OutboxPublisher>("outbox-publisher")
    .WithReference(serviceBus)
    .WithReference(postgresDb);

builder
    .AddProject<Projects.eMechanic_NotificationService_API>("eMechanic-NotificationService")
    .WithReference(serviceBus);

await builder.Build().RunAsync();
