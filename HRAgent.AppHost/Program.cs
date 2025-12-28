var builder = DistributedApplication.CreateBuilder(args);

// Add MongoDB container for local development
// Aspire automatically configures authentication with default credentials
var mongodb = builder.AddMongoDB("mongodb")
    .WithConnectionProperty("authSource", "admin");
    // .WithDataVolume();  // Persist data across container restarts

var database = mongodb.AddDatabase("hragent");

// Add Azurite blob storage emulator with automatic container management
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var blobs = storage.AddBlobs("blobs");

// Add backend API project with MongoDB and Blob Storage references
var backend = builder.AddProject<Projects.HRAgent_Api>("backend")
    .WithReference(database) // ✅ Aspire injects MongoDB connection string automatically
    .WithReference(blobs)    // ✅ Aspire injects Blob Storage connection string automatically
    .WaitFor(mongodb)
    .WaitFor(storage)
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
