var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.eMechanic_API>("emechanic-api");

builder.Build().Run();
