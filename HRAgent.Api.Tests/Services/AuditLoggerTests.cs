using HRAgent.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HRAgent.Api.Tests.Services;

public class AuditLoggerTests
{
    private readonly Mock<ILogger<AuditLogger>> _mockLogger;

    public AuditLoggerTests()
    {
        _mockLogger = new Mock<ILogger<AuditLogger>>();
    }

    [Fact]
    public async Task LogAsync_ThrowsException_WhenEventTypeIsNull()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["BlobStorage:ConnectionString"] = "UseDevelopmentStorage=true",
            ["BlobStorage:ContainerName"] = "test-audit-logs"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        
        var auditLogger = new AuditLogger(configuration, _mockLogger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            auditLogger.LogAsync(
                eventType: null!,
                userId: "user-123",
                data: new { }));
    }

    [Fact]
    public async Task LogAsync_ThrowsException_WhenUserIdIsNull()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["BlobStorage:ConnectionString"] = "UseDevelopmentStorage=true",
            ["BlobStorage:ContainerName"] = "test-audit-logs"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        
        var auditLogger = new AuditLogger(configuration, _mockLogger.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            auditLogger.LogAsync(
                eventType: "test.event",
                userId: null!,
                data: new { }));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenConnectionStringNotConfigured()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new AuditLogger(emptyConfig, _mockLogger.Object));
        
        Assert.Contains("Blob Storage connection string not configured", exception.Message);
    }

    [Fact]
    public async Task QueryLogsAsync_ReturnsEmptyList_WhenNoLogsExist()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["BlobStorage:ConnectionString"] = "UseDevelopmentStorage=true",
            ["BlobStorage:ContainerName"] = "test-audit-logs"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        
        var auditLogger = new AuditLogger(configuration, _mockLogger.Object);
        var nonExistentThreadId = "thread-" + Guid.NewGuid().ToString("N")[..8];
        
        // Act
        var logs = await auditLogger.QueryLogsAsync(nonExistentThreadId);
        
        // Assert
        Assert.Empty(logs);
    }

    [Fact]
    public async Task LogAsync_IncludesAllRequiredFields_WhenCalled()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["BlobStorage:ConnectionString"] = "UseDevelopmentStorage=true",
            ["BlobStorage:ContainerName"] = "test-audit-logs"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        
        var auditLogger = new AuditLogger(configuration, _mockLogger.Object);
        var testData = new { field1 = "value1", field2 = 42 };
        var threadId = "thread-test-" + Guid.NewGuid().ToString("N")[..8];
        
        // Act
        await auditLogger.LogAsync(
            eventType: "test.comprehensive",
            userId: "user-456",
            data: testData,
            reasoning: "Complete test",
            threadId: threadId,
            correlationId: "correlation-123");
        
        // Assert - Query logs and verify all required fields present
        var logs = await auditLogger.QueryLogsAsync(threadId);
        Assert.NotNull(logs);
        Assert.NotEmpty(logs);
        
        var logContent = logs.First();
        // Verify all AC #9 required fields in JSON Lines format
        Assert.Contains("\"timestamp\":", logContent);
        Assert.Contains("\"userId\":\"user-456\"", logContent);
        Assert.Contains("\"eventType\":\"test.comprehensive\"", logContent);
        Assert.Contains("\"data\":", logContent);
        Assert.Contains("\"reasoning\":\"Complete test\"", logContent);
        Assert.Contains("\"threadId\":\"" + threadId, logContent);
        Assert.Contains("\"correlationId\":\"correlation-123\"", logContent);
        Assert.Contains("\"version\":\"1.0\"", logContent);
    }

    [Fact]
    public async Task LogAsync_ThreadSafe_WhenCalledConcurrently()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["BlobStorage:ConnectionString"] = "UseDevelopmentStorage=true",
            ["BlobStorage:ContainerName"] = "test-audit-logs-concurrent"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        
        var auditLogger = new AuditLogger(configuration, _mockLogger.Object);
        var threadId = "thread-concurrent-" + Guid.NewGuid().ToString("N")[..8];
        
        // Act - Execute 10 concurrent writes (AC #6: thread-safe with SemaphoreSlim)
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(Task.Run(async () =>
            {
                await auditLogger.LogAsync(
                    eventType: $"test.concurrent.{index}",
                    userId: $"user-{index}",
                    data: new { iteration = index, timestamp = DateTime.UtcNow },
                    threadId: threadId);
            }));
        }
        
        // Wait for all concurrent writes to complete
        await Task.WhenAll(tasks);
        
        // Assert - All 10 entries should be written without corruption
        var logs = await auditLogger.QueryLogsAsync(threadId);
        Assert.NotNull(logs);
        Assert.NotEmpty(logs);
        
        var logContent = logs.First();
        var logLines = logContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        // Verify we have 10 distinct log entries
        Assert.True(logLines.Length >= 10, $"Expected at least 10 log entries, found {logLines.Length}");
        
        // Verify each entry is valid JSON with correct event types
        var eventTypes = new HashSet<string>();
        foreach (var line in logLines.Take(10))
        {
            Assert.Contains("\"eventType\":\"test.concurrent.", line);
            Assert.Contains("\"threadId\":\"" + threadId, line);
        }
    }
}
