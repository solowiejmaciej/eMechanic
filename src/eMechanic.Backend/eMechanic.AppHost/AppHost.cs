var builder = DistributedApplication.CreateBuilder(args);

var postgresServer = builder
    .AddPostgres("emechanic-postgres-server")
    .WithContainerName("emechanic-postgres")
    .WithDataVolume()
    .WithHostPort(5433)
    .WithLifetime(ContainerLifetime.Persistent);

var postgresDb = postgresServer.AddDatabase("eMechanic");

var redisCache = builder.AddRedis("emechanic-cache");

builder
    .AddProject<Projects.eMechanic_API>("emechanic-api")
    .WithReference(postgresDb)
    .WithReference(redisCache)
    .WaitFor(postgresServer);

builder.Build().Run();
