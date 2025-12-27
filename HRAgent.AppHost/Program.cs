var builder = DistributedApplication.CreateBuilder(args);

// Add backend API project
var backend = builder.AddProject<Projects.HRAgent_Api>("backend")
    .WithExternalHttpEndpoints();

// Add frontend Vite app (AddNpmApp deprecated in Aspire 13.0)

// var frontend = builder.AddViteApp("frontend", "../hragent-ui")
//     .WithExternalHttpEndpoints()
//     .WithEnvironment("VITE_API_URL", backend.GetEndpoint("https"));

var frontend = builder.AddViteApp("frontend", "../hragent-ui")
                    .WithReference(backend)
                    .WaitFor(backend)
                    .WithEndpoint(endpointName: "http", endpoint =>
                    {
                        endpoint.Port = builder.ExecutionContext.IsRunMode ?
                                        5173 : null;
                    });

builder.Build().Run();
