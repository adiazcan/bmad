using HRAgent.Api.Data;
using HRAgent.Api.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace HRAgent.Api.Tests.Services;

/// <summary>
/// Integration tests for PatternRepository using Testcontainers.MongoDb
/// </summary>
public class PatternRepositoryTests : IAsyncLifetime
{
    private MongoDbContainer? _mongoContainer;
    private IMongoClient? _mongoClient;
    private MongoDbService? _mongoService;
    private PatternRepository? _repository;

    public async Task InitializeAsync()
    {
        // Start MongoDB container
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .Build();

        await _mongoContainer.StartAsync();

        // Create MongoDB client and service
        _mongoClient = new MongoClient(_mongoContainer.GetConnectionString());
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDB:DatabaseName"] = "test-hragent"
            })
            .Build();

        _mongoService = new MongoDbService(_mongoClient, config);
        _repository = new PatternRepository(_mongoService);

        // Create index
        var collection = _mongoService.Database.GetCollection<UserPattern>("user-patterns");
        var indexKeys = Builders<UserPattern>.IndexKeys.Ascending(p => p.UserId);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<UserPattern>(indexKeys));
    }

    public async Task DisposeAsync()
    {
        if (_mongoContainer != null)
        {
            await _mongoContainer.DisposeAsync();
        }
    }

    [Fact]
    public async Task UpsertAsync_InsertsNewPattern()
    {
        // Arrange
        var pattern = new UserPattern
        {
            UserId = "user-123",
            PatternType = "timesheet",
            PatternData = "{\"dayOfWeek\":5,\"time\":\"09:00\"}"
        };

        // Act
        await _repository!.UpsertAsync(pattern);

        // Assert
        var retrieved = await _repository.GetByUserIdAsync("user-123");
        Assert.NotNull(retrieved);
        Assert.Equal("timesheet", retrieved.PatternType);
    }

    [Fact]
    public async Task UpsertAsync_UpdatesExistingPattern()
    {
        // Arrange
        var pattern = new UserPattern
        {
            UserId = "user-456",
            PatternType = "timesheet",
            PatternData = "{\"dayOfWeek\":1}"
        };
        await _repository!.UpsertAsync(pattern);

        // Modify pattern
        pattern.PatternData = "{\"dayOfWeek\":5}"; // Change to Friday

        // Act
        await _repository.UpsertAsync(pattern);

        // Assert
        var retrieved = await _repository.GetByUserIdAsync("user-456");
        Assert.NotNull(retrieved);
        Assert.Contains("5", retrieved.PatternData);

        // Verify only one document exists
        var collection = _mongoService!.Database.GetCollection<UserPattern>("user-patterns");
        var count = await collection.CountDocumentsAsync(p => p.UserId == "user-456");
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsNullWhenNotFound()
    {
        // Act
        var result = await _repository!.GetByUserIdAsync("non-existent-user");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpsertAsync_UpdatesUpdatedAtTimestamp()
    {
        // Arrange
        var pattern = new UserPattern
        {
            UserId = "user-789",
            PatternType = "reminder",
            PatternData = "{\"frequency\":\"daily\"}"
        };
        await _repository!.UpsertAsync(pattern);

        var first = await _repository.GetByUserIdAsync("user-789");
        var firstUpdatedAt = first!.UpdatedAt;

        // Wait to ensure timestamp difference
        await Task.Delay(100);

        // Act
        pattern.PatternData = "{\"frequency\":\"weekly\"}";
        await _repository.UpsertAsync(pattern);

        // Assert
        var second = await _repository.GetByUserIdAsync("user-789");
        Assert.True(second!.UpdatedAt > firstUpdatedAt);
    }
}
