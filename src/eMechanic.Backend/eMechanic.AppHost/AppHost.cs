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
var azureStorage = builder.AddConnectionString("Storage");
var emailAppID = builder.AddParameter("EmailAppID", secret: true);
var emailSecret = builder.AddParameter("EmailLabsServiceKey", secret: true);

builder
    .AddProject<Projects.eMechanic_API>("eMechanic-Core")
    .WithReference(postgresDb)
    .WithReference(redisCache)
    .WithReference(azureStorage)
    .WaitFor(postgresServer);

builder.AddAzureFunctionsProject<Projects.eMechanic_OutboxPublisher>("outbox-publisher")
    .WithReference(serviceBus)
    .WithReference(postgresDb);

builder
    .AddProject<Projects.eMechanic_NotificationService_API>("eMechanic-NotificationService")
    .WithReference(serviceBus)
    .WithEnvironment("Notifications__EmailAppID", emailAppID)
    .WithEnvironment("Notifications__EmailSecret", emailSecret);


await builder.Build().RunAsync();
