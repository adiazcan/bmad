var builder = DistributedApplication.CreateBuilder(args);

// Add Cosmos DB emulator with Docker container (Linux emulator)
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .RunAsEmulator(configureContainer: container =>
    {
        container.WithImageRegistry("mcr.microsoft.com")
            .WithImage("cosmosdb/linux/azure-cosmos-emulator")
            .WithImageTag("vnext-preview");
    });

var database = cosmos.AddCosmosDatabase("hragent");

// Add Azurite blob storage emulator with automatic container management
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var blobs = storage.AddBlobs("blobs");

// Add backend API project with Cosmos DB and Blob Storage references
var backend = builder.AddProject<Projects.HRAgent_Api>("backend")
    .WithReference(database) // ✅ Aspire injects Cosmos DB connection string automatically
    .WithReference(blobs)    // ✅ Aspire injects Blob Storage connection string automatically
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
