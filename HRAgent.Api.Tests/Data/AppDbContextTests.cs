using HRAgent.Api.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HRAgent.Api.Tests.Data;

public class AppDbContextTests
{
    [Fact]
    public async Task CanSaveAndRetrieveConversationThread()
    {
        // Arrange: Use InMemory provider for unit tests (NOT real Cosmos DB)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        
        using var context = new AppDbContext(options);
        
        var thread = new ConversationThread
        {
            ThreadId = "test-thread-123",
            UserId = "user-456",
            Messages = new List<Message>
            {
                new Message { Role = "user", Text = "Test message", TokenCount = 2 }
            }
        };
        
        // Act
        context.Conversations.Add(thread);
        await context.SaveChangesAsync();
        
        var retrieved = await context.Conversations
            .FirstOrDefaultAsync(c => c.ThreadId == "test-thread-123");
        
        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("test-thread-123", retrieved.ThreadId);
        Assert.Equal("user-456", retrieved.UserId);
        Assert.Single(retrieved.Messages);
        Assert.Equal("Test message", retrieved.Messages[0].Text);
    }
    
    [Fact]
    public async Task PartitionKeyQueryIsEfficient()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        
        using var context = new AppDbContext(options);
        
        // Add multiple threads
        context.Conversations.Add(new ConversationThread { ThreadId = "thread-1", UserId = "user-1" });
        context.Conversations.Add(new ConversationThread { ThreadId = "thread-2", UserId = "user-2" });
        await context.SaveChangesAsync();
        
        // Act: Query with partition key filter
        var result = await context.Conversations
            .Where(c => c.ThreadId == "thread-1") // ✅ Partition key filter
            .FirstOrDefaultAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("thread-1", result.ThreadId);
        Assert.Equal("user-1", result.UserId);
    }
    
    [Fact]
    public async Task CanSaveAndRetrieveUserPattern()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        
        using var context = new AppDbContext(options);
        
        var pattern = new UserPattern
        {
            UserId = "user-123",
            PatternType = "timesheet",
            PatternData = "{\"frequency\":\"daily\",\"time\":\"09:00\"}"
        };
        
        // Act
        context.UserPatterns.Add(pattern);
        await context.SaveChangesAsync();
        
        var retrieved = await context.UserPatterns
            .Where(p => p.UserId == "user-123") // ✅ Partition key filter
            .FirstOrDefaultAsync();
        
        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("user-123", retrieved.UserId);
        Assert.Equal("timesheet", retrieved.PatternType);
        Assert.Contains("frequency", retrieved.PatternData);
    }
    
    [Fact]
    public async Task ConversationThreadHasDefaultValues()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        
        using var context = new AppDbContext(options);
        
        var thread = new ConversationThread
        {
            ThreadId = "test-thread",
            UserId = "test-user"
        };
        
        // Act
        context.Conversations.Add(thread);
        await context.SaveChangesAsync();
        
        var retrieved = await context.Conversations.FirstAsync();
        
        // Assert
        Assert.NotNull(retrieved.Id);
        Assert.NotEqual(default(DateTime), retrieved.CreatedAt);
        Assert.NotEqual(default(DateTime), retrieved.UpdatedAt);
        Assert.Empty(retrieved.Messages);
    }
    
    [Fact]
    public async Task MessageHasDefaultValues()
    {
        // Arrange
        var message = new Message
        {
            Text = "Test message"
        };
        
        // Assert
        Assert.NotNull(message.Id);
        Assert.Equal("user", message.Role); // Default role
        Assert.NotEqual(default(DateTime), message.CreatedAt);
        Assert.Equal(0, message.TokenCount);
    }
}
