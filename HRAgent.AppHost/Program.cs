var builder = DistributedApplication.CreateBuilder(args);

// Add backend API project
var backend = builder.AddProject<Projects.HRAgent_Api>("backend")
    .WithExternalHttpEndpoints();

// Add frontend Vite app (AddNpmApp deprecated in Aspire 13.0)
var frontend = builder.AddViteApp("frontend", "../hragent-ui")
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", backend.GetEndpoint("https"));

builder.Build().Run();
