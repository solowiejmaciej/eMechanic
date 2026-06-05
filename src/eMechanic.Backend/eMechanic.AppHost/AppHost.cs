using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var logger = loggerFactory.CreateLogger("AppHost");

var externalPostgresConnectionString = builder.Configuration.GetConnectionString("eMechanic");
var useExternalPostgres = !string.IsNullOrWhiteSpace(externalPostgresConnectionString);

IResourceBuilder<IResourceWithConnectionString> postgresDb;

if (useExternalPostgres)
{
    logger.LogInformation("[AppHost] Using external PostgreSQL from ConnectionStrings:eMechanic");
    postgresDb = builder.AddConnectionString("eMechanic");
}
else
{
    logger.LogInformation("[AppHost] No external PostgreSQL configured — starting local container");
    var postgresServer = builder
        .AddPostgres("emechanic-postgres-server")
        .WithContainerName("emechanic-postgres")
        .WithDataVolume()
        .WithHostPort(5433)
        .WithLifetime(ContainerLifetime.Persistent);

    postgresDb = postgresServer.AddDatabase("eMechanic");
}

var redisCache = builder.AddRedis("emechanic-cache");
var serviceBus = builder.AddConnectionString("AzureServiceBus");
var azureStorage = builder.AddConnectionString("Storage");
var googleApiKey = builder.AddParameter("google-api-key", secret: true);
var googleClientId = builder.AddParameter("google-client-id", secret: true);
var stripeKey = builder.AddParameter("stripe-key", secret: true);
var stripeWebhookSecret = builder.AddParameter("stripe-webhook-secret", secret: true);

var apiProject = builder
    .AddProject<Projects.eMechanic_API>("eMechanic-Core")
    .WithReference(postgresDb)
    .WithReference(redisCache)
    .WithReference(azureStorage)
    .WithEnvironment("LLMProviders__Google__ApiKey", googleApiKey)
    .WithEnvironment("Authentication__Google__ClientId", googleClientId)
    .WithEnvironment("Stripe__SecretKey", stripeKey)
    .WithEnvironment("Stripe__WebhookSecret", stripeWebhookSecret);

if (!useExternalPostgres)
{
    apiProject.WaitFor(postgresDb);
}

var outboxPublisher = builder.AddAzureFunctionsProject<Projects.eMechanic_OutboxPublisher>("outbox-publisher")
    .WithReference(serviceBus)
    .WithReference(postgresDb);

if (!useExternalPostgres)
{
    outboxPublisher.WaitFor(postgresDb);
}

builder
    .AddProject<Projects.eMechanic_NotificationService_API>("eMechanic-NotificationService")
    .WithReference(serviceBus);

await builder.Build().RunAsync();
