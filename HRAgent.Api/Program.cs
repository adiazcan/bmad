using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;
using HRAgent.Api.Data;
using HRAgent.Api.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add authorization services
builder.Services.AddAuthorization();

// Add DbContext with Cosmos DB provider
// Connection string injected by Aspire via WithReference(database) or manual appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Aspire injects as "ConnectionStrings:hragent" when using WithReference(database)
    // Fallback to manual "CosmosDb:ConnectionString" for standalone runs
    var connectionString = builder.Configuration.GetConnectionString("hragent")
        ?? builder.Configuration["CosmosDb:ConnectionString"]
        ?? throw new InvalidOperationException("Cosmos DB connection string not configured");
    
    var databaseName = builder.Configuration["CosmosDb:DatabaseName"]
        ?? "hragent";
    
    options.UseCosmos(
        connectionString: connectionString,
        databaseName: databaseName,
        cosmosOptionsAction: cosmosOptions =>
        {
            // Enable automatic container creation
            cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
            
            // Request timeout (30 seconds default)
            cosmosOptions.RequestTimeout(TimeSpan.FromSeconds(30));
        });
});

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:5173" };  // Fallback to Vite dev server
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register AuditLogger as singleton (shared across all requests)
// ✅ Singleton ensures single SemaphoreSlim instance for thread safety
builder.Services.AddSingleton<AuditLogger>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add health checks for Cosmos DB and Blob Storage with connectivity verification
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        name: "cosmos-db",
        tags: new[] { "db", "cosmos" },
        customTestQuery: async (db, cancellationToken) =>
        {
            // Verify actual Cosmos DB connectivity
            return await db.Database.CanConnectAsync(cancellationToken);
        })
    .AddAzureBlobStorage(
        builder.Configuration.GetConnectionString("blobs") 
            ?? builder.Configuration["BlobStorage:ConnectionString"]!,
        name: "blob-storage",
        tags: new[] { "storage", "audit" });

var app = builder.Build();

// Ensure database and containers are created
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync(); // ✅ Creates DB + containers if missing
        Console.WriteLine("✅ Cosmos DB database and containers ensured");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to connect to Cosmos DB: {ex.Message}");
        Console.WriteLine("   Ensure Cosmos DB emulator is running or connection string is correct");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

// CRITICAL ORDER: CORS -> Authentication -> Authorization
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoints
app.MapHealthChecks("/health"); // Basic health check - all checks
app.MapHealthChecks("/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("storage") // DB + Storage checks
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Protected test endpoint
app.MapGet("/secure", () => new { message = "Authenticated!", timestamp = DateTime.UtcNow })
    .RequireAuthorization()
    .WithName("GetSecure");

// Test endpoint to verify Cosmos DB connectivity (dev only)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/test-cosmos", async (AppDbContext db) =>
{
    try
    {
        // Create a test conversation thread
        var thread = new ConversationThread
        {
            ThreadId = Guid.NewGuid().ToString(),
            UserId = "test-user",
            Messages = new List<Message>
            {
                new Message
                {
                    Role = "user",
                    Text = "Hello, HRAgent!",
                    TokenCount = 4
                }
            }
        };
        
        db.Conversations.Add(thread);
        await db.SaveChangesAsync();
        
        // Retrieve the thread (using partition key)
        var retrieved = await db.Conversations
            .Where(c => c.ThreadId == thread.ThreadId) // ✅ Filters by partition key
            .FirstOrDefaultAsync();
        
        if (retrieved != null)
        {
            return Results.Ok(new 
            { 
                success = true, 
                message = "Cosmos DB connected successfully",
                threadId = retrieved.ThreadId,
                messageCount = retrieved.Messages.Count
            });
        }
        
        return Results.Problem("Failed to retrieve test conversation");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Cosmos DB error: {ex.Message}");
    }
})
.WithName("TestCosmos");

    app.MapPost("/test-audit", async (AuditLogger auditLogger) =>
    {
        try
        {
            // Write test audit log
            await auditLogger.LogAsync(
                eventType: "test.audit.log",
                userId: "test-user-123",
                data: new { message = "Test audit log entry", timestamp = DateTime.UtcNow },
                reasoning: "Manual test of audit logging system",
                threadId: "test-thread-" + Guid.NewGuid().ToString("N")[..8]);

            return Results.Ok(new 
            { 
                success = true, 
                message = "Audit log written successfully to Blob Storage"
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Audit logging failed: {ex.Message}");
        }
    })
    .WithName("TestAudit");

    app.MapGet("/test-audit/{threadId}", async (string threadId, AuditLogger auditLogger) =>
    {
        try
        {
            var logs = await auditLogger.QueryLogsAsync(threadId);
            return Results.Ok(new 
            { 
                success = true, 
                threadId,
                logCount = logs.Count,
                logs = logs.SelectMany(l => l.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                    .Select(line => JsonSerializer.Deserialize<JsonElement>(line))
                    .ToList()
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Query failed: {ex.Message}");
        }
    })
    .WithName("QueryAuditLogs");
}

// Query conversation by threadId (demonstrates partition key usage)
app.MapGet("/conversations/{threadId}", async (string threadId, AppDbContext db) =>
{
    var thread = await db.Conversations
        .Where(c => c.ThreadId == threadId) // ✅ Partition key filter
        .FirstOrDefaultAsync();
    
    if (thread == null)
    {
        return Results.NotFound(new { error = "Conversation not found" });
    }
    
    return Results.Ok(thread);
})
.WithName("GetConversation");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}