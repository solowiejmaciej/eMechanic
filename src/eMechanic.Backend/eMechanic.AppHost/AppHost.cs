var builder = DistributedApplication.CreateBuilder(args);

var postgresServer = builder
    .AddPostgres("emechanic-postgres-server")
    .WithContainerName("emechanic-postgres")
    .WithDataVolume()
    .WithHostPort(5433);

var postgresDb = postgresServer.AddDatabase("eMechanic");

builder
    .AddProject<Projects.eMechanic_API>("emechanic-api")
    .WithReference(postgresDb)
    .WaitFor(postgresServer);

builder.Build().Run();
