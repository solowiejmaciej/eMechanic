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
var googleApiKey = builder.AddParameter("google-api-key", secret: true);
var googleClientId = builder.AddParameter("google-client-id", secret: true);
var stripeKey = builder.AddParameter("stripe-key", secret: true);
var stripeWebhookSecret = builder.AddParameter("stripe-webhook-secret", secret: true);

builder
    .AddProject<Projects.eMechanic_API>("eMechanic-Core")
    .WithReference(postgresDb)
    .WithReference(redisCache)
    .WithReference(azureStorage)
    .WithEnvironment("LLMProviders__Google__ApiKey", googleApiKey)
    .WithEnvironment("Authentication__Google__ClientId", googleClientId)
    .WithEnvironment("Stripe__SecretKey", stripeKey)
    .WithEnvironment("Stripe__WebhookSecret", stripeWebhookSecret)
    .WaitFor(postgresServer);

builder.AddAzureFunctionsProject<Projects.eMechanic_OutboxPublisher>("outbox-publisher")
    .WithReference(serviceBus)
    .WithReference(postgresDb);

builder
    .AddProject<Projects.eMechanic_NotificationService_API>("eMechanic-NotificationService")
    .WithReference(serviceBus);

await builder.Build().RunAsync();
