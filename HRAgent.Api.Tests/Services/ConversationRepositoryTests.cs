using HRAgent.Api.Data;
using HRAgent.Api.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace HRAgent.Api.Tests.Services;

/// <summary>
/// Integration tests for ConversationRepository using Testcontainers.MongoDb
/// RED-GREEN-REFACTOR: Write failing tests first, then implement
/// </summary>
public class ConversationRepositoryTests : IAsyncLifetime
{
    private MongoDbContainer? _mongoContainer;
    private IMongoClient? _mongoClient;
    private MongoDbService? _mongoService;
    private ConversationRepository? _repository;

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
        _repository = new ConversationRepository(_mongoService);

        // Create index
        var collection = _mongoService.Database.GetCollection<ConversationThread>("conversations");
        var indexKeys = Builders<ConversationThread>.IndexKeys.Ascending(c => c.ThreadId);
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<ConversationThread>(indexKeys));
    }

    public async Task DisposeAsync()
    {
        if (_mongoContainer != null)
        {
            await _mongoContainer.DisposeAsync();
        }
    }

    [Fact]
    public async Task AddAsync_SavesConversationThread()
    {
        // Arrange
        var thread = new ConversationThread
        {
            ThreadId = "test-thread-1",
            UserId = "user-123",
            Messages = new List<Message>
            {
                new Message { Role = "user", Text = "Hello", TokenCount = 1 }
            }
        };

        // Act
        await _repository!.AddAsync(thread);

        // Assert
        var retrieved = await _repository.GetByThreadIdAsync("test-thread-1");
        Assert.NotNull(retrieved);
        Assert.Equal("test-thread-1", retrieved.ThreadId);
        Assert.Equal("user-123", retrieved.UserId);
        Assert.Single(retrieved.Messages);
    }

    [Fact]
    public async Task GetByThreadIdAsync_ReturnsNullWhenNotFound()
    {
        // Act
        var result = await _repository!.GetByThreadIdAsync("non-existent-thread");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsAllUserThreads()
    {
        // Arrange
        await _repository!.AddAsync(new ConversationThread { ThreadId = "thread-1", UserId = "user-1" });
        await _repository.AddAsync(new ConversationThread { ThreadId = "thread-2", UserId = "user-1" });
        await _repository.AddAsync(new ConversationThread { ThreadId = "thread-3", UserId = "user-2" });

        // Act
        var threads = await _repository.GetByUserIdAsync("user-1");

        // Assert
        Assert.Equal(2, threads.Count);
        Assert.All(threads, t => Assert.Equal("user-1", t.UserId));
    }

    [Fact]
    public async Task UpdateAsync_UpsertNewThread()
    {
        // Arrange
        var thread = new ConversationThread
        {
            ThreadId = "new-thread",
            UserId = "user-new",
            Messages = new List<Message> { new Message { Text = "New message" } }
        };

        // Act
        await _repository!.UpdateAsync(thread);

        // Assert
        var retrieved = await _repository.GetByThreadIdAsync("new-thread");
        Assert.NotNull(retrieved);
        Assert.Equal("user-new", retrieved.UserId);
    }

    [Fact]
    public async Task UpdateAsync_ReplacesExistingThread()
    {
        // Arrange
        var thread = new ConversationThread
        {
            ThreadId = "existing-thread",
            UserId = "user-old",
            Messages = new List<Message> { new Message { Text = "Old message" } }
        };
        await _repository!.AddAsync(thread);

        // Modify thread
        thread.UserId = "user-new";
        thread.Messages.Add(new Message { Text = "New message" });

        // Act
        await _repository.UpdateAsync(thread);

        // Assert
        var retrieved = await _repository.GetByThreadIdAsync("existing-thread");
        Assert.NotNull(retrieved);
        Assert.Equal("user-new", retrieved.UserId);
        Assert.Equal(2, retrieved.Messages.Count);
    }

    [Fact]
    public async Task DeleteAsync_RemovesThread()
    {
        // Arrange
        await _repository!.AddAsync(new ConversationThread { ThreadId = "delete-me", UserId = "user-1" });

        // Act
        await _repository.DeleteAsync("delete-me");

        // Assert
        var retrieved = await _repository.GetByThreadIdAsync("delete-me");
        Assert.Null(retrieved);
    }
}
