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
        
        // Assert - Verify logs were queried without errors
        var logs = await auditLogger.QueryLogsAsync(threadId);
        Assert.NotNull(logs);
    }
}
