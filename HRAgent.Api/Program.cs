using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using MongoDB.Driver;
using HRAgent.Api.Data;
using HRAgent.Api.Services;
using HRAgent.Api.Clients;
using System.Text.Json;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add authorization services
builder.Services.AddAuthorization();

// Add MongoDB client as singleton (connection pooling managed internally)
// Connection string injected by Aspire via WithReference(database) or manual appsettings.json
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    // Aspire injects as "ConnectionStrings:hragent" when using WithReference(database)
    // Fallback to manual "MongoDB:ConnectionString" for standalone runs
    var connectionString = builder.Configuration.GetConnectionString("hragent")
        ?? builder.Configuration["MongoDB:ConnectionString"]
        ?? throw new InvalidOperationException("MongoDB connection string not configured");
    
    Console.WriteLine($"🔗 MongoDB Connection String: {connectionString}");

    var settings = MongoClientSettings.FromConnectionString(connectionString);
    
    // Optional: Configure connection pool
    settings.MaxConnectionPoolSize = 100;
    settings.MinConnectionPoolSize = 10;
    settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(2);
    settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    
    return new MongoClient(settings);
});

// Add MongoDbService as scoped (per request)
builder.Services.AddScoped<MongoDbService>();

// Register MongoDB repositories as scoped services
builder.Services.AddScoped<ConversationRepository>();
builder.Services.AddScoped<PatternRepository>();

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

// Configure Factorial options from appsettings.json
builder.Services.Configure<FactorialOptions>(
    builder.Configuration.GetSection("Factorial"));

// Register FactorialClient with HttpClient factory + Polly policies
builder.Services.AddHttpClient<FactorialClient>()
    // Timeout policy - 2 seconds per request (innermost wrapper)
    .AddPolicyHandler((services, request) =>
    {
        var logger = services.GetRequiredService<ILogger<FactorialClient>>();
        return Policy.TimeoutAsync<HttpResponseMessage>(
            timeout: TimeSpan.FromSeconds(2),
            onTimeoutAsync: (context, timespan, task) =>
            {
                logger.LogWarning("Factorial API request timeout after {Timeout}s", 
                    timespan.TotalSeconds);
                return Task.CompletedTask;
            });
    })
    
    // Circuit breaker policy - shared singleton instance
    .AddPolicyHandler((services, request) =>
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var circuitBreakerLogger = loggerFactory.CreateLogger("FactorialClient.CircuitBreaker");

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<Polly.Timeout.TimeoutRejectedException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, duration) =>
                {
                    circuitBreakerLogger.LogError(
                        "Factorial API circuit breaker OPEN for {Duration}s: {Error}",
                        duration.TotalSeconds, 
                        outcome.Exception?.Message ?? "Unknown error");
                },
                onReset: () =>
                {
                    circuitBreakerLogger.LogInformation(
                        "Factorial API circuit breaker CLOSED - service recovered");
                },
                onHalfOpen: () =>
                {
                    circuitBreakerLogger.LogWarning(
                        "Factorial API circuit breaker HALF-OPEN - testing recovery");
                });
    })
    
    // Retry policy - exponential backoff with jitter (outermost wrapper)
    .AddPolicyHandler((services, request) =>
    {
        var logger = services.GetRequiredService<ILogger<FactorialClient>>();
        return HttpPolicyExtensions
            .HandleTransientHttpError()  // 5xx, 408, 429
            .Or<Polly.Timeout.TimeoutRejectedException>()  // Polly timeout
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => 
                    TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100)  // 100ms, 200ms, 400ms
                    + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 50)), // Jitter ±25ms
                onRetry: (outcome, timespan, attemptNumber, context) =>
                {
                    logger.LogWarning(
                        "Factorial API retry {Attempt}/3 after {Delay}ms: {Error}",
                        attemptNumber, 
                        timespan.TotalMilliseconds,
                        outcome.Exception?.Message ?? "Unknown error");
                });
    });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add health checks for MongoDB and Blob Storage with connectivity verification
var blobConnectionString = builder.Configuration.GetConnectionString("blobs") 
    ?? builder.Configuration["BlobStorage:ConnectionString"]
    ?? (builder.Environment.IsEnvironment("Testing") ? "UseDevelopmentStorage=true" : null);

if (string.IsNullOrEmpty(blobConnectionString))
{
    throw new InvalidOperationException(
        "Blob Storage connection string not configured. " +
        "Ensure 'ConnectionStrings:blobs' (Aspire) or 'BlobStorage:ConnectionString' is set.");
}

builder.Services.AddHealthChecks()
    .AddMongoDb(
        sp => sp.GetRequiredService<IMongoClient>(),
        name: "mongodb",
        tags: new[] { "db", "mongodb" })
    .AddAzureBlobStorage(
        blobConnectionString,
        name: "blob-storage",
        tags: new[] { "storage", "audit" })
    .AddCheck<FactorialHealthCheck>("factorial-api", tags: new[] { "external", "factorial" });

var app = builder.Build();

// Ensure database and collections are created with indexes (skip in testing)
if (!app.Environment.IsEnvironment("Testing"))
{
using (var scope = app.Services.CreateScope())
{
    try
    {
        var mongoService = scope.ServiceProvider.GetRequiredService<MongoDbService>();
        var database = mongoService.Database;
        
        // Create conversations collection with threadId index
        var conversationsCollection = database.GetCollection<ConversationThread>("conversations");
        var conversationsIndexKeys = Builders<ConversationThread>.IndexKeys.Ascending(c => c.ThreadId);
        var conversationsIndexModel = new CreateIndexModel<ConversationThread>(conversationsIndexKeys);
        await conversationsCollection.Indexes.CreateOneAsync(conversationsIndexModel);
        
        // Create user-patterns collection with userId index
        var patternsCollection = database.GetCollection<UserPattern>("user-patterns");
        var patternsIndexKeys = Builders<UserPattern>.IndexKeys.Ascending(p => p.UserId);
        var patternsIndexModel = new CreateIndexModel<UserPattern>(patternsIndexKeys);
        await patternsCollection.Indexes.CreateOneAsync(patternsIndexModel);
        
        Console.WriteLine("✅ MongoDB database and collections ensured with indexes");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to connect to MongoDB: {ex.Message}");
        Console.WriteLine("   Ensure MongoDB container is running or connection string is correct");
        throw;
    }
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
    Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("storage") || check.Tags.Contains("external") // DB + Storage + External API checks
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

// Test endpoint to verify MongoDB connectivity (dev only)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/test-mongodb", async (MongoDbService mongoService) =>
{
    try
    {
        var collection = mongoService.Database.GetCollection<ConversationThread>("conversations");
        
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
        
        await collection.InsertOneAsync(thread);
        
        // Retrieve the thread using threadId index
        var retrieved = await collection
            .Find(c => c.ThreadId == thread.ThreadId)
            .FirstOrDefaultAsync();
        
        if (retrieved != null)
        {
            return Results.Ok(new 
            { 
                success = true, 
                message = "MongoDB connected successfully",
                threadId = retrieved.ThreadId,
                messageCount = retrieved.Messages.Count
            });
        }
        
        return Results.Problem("Failed to retrieve test conversation");
    }
    catch (Exception ex)
    {
        return Results.Problem($"MongoDB error: {ex.Message}");
    }
})
.WithName("TestMongoDB");

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

    app.MapGet("/test-factorial", async (FactorialClient factorialClient) =>
    {
        try
        {
            // Test with real demo employee ID from Factorial demo environment
            var employee = await factorialClient.GetEmployeeAsync("1779508");
            
            return Results.Ok(new 
            { 
                success = true, 
                message = "Factorial API client configured successfully",
                employee = employee
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Factorial API test failed: {ex.Message}");
        }
    })
    .WithName("TestFactorial");
}

// Query conversation by threadId (demonstrates indexed query)
app.MapGet("/conversations/{threadId}", async (string threadId, MongoDbService mongoService) =>
{
    var collection = mongoService.Database.GetCollection<ConversationThread>("conversations");
    
    var thread = await collection
        .Find(c => c.ThreadId == threadId) // ✅ Uses threadId index
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