var builder = DistributedApplication.CreateBuilder(args);

// Add Cosmos DB emulator with automatic container management
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .RunAsEmulator();

var database = cosmos.AddCosmosDatabase("hragent");

// Add backend API project with Cosmos DB reference
var backend = builder.AddProject<Projects.HRAgent_Api>("backend")
    .WithReference(database) // ✅ Aspire injects connection string automatically
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
