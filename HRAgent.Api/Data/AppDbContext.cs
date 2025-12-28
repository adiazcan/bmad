using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace HRAgent.Api.Data;

/// <summary>
/// Entity Framework Core DbContext for Cosmos DB Serverless.
/// Manages conversation threads and user patterns.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for entity collections
    public DbSet<ConversationThread> Conversations => Set<ConversationThread>();
    public DbSet<UserPattern> UserPatterns => Set<UserPattern>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Conversations container configuration
        modelBuilder.Entity<ConversationThread>(entity =>
        {
            entity.ToContainer("conversations");
            entity.HasPartitionKey(e => e.ThreadId); // ✅ Critical: partition by threadId
            entity.HasKey(e => e.Id);
            
            // Property mappings to camelCase JSON
            entity.Property(e => e.Id).ToJsonProperty("id");
            entity.Property(e => e.ThreadId).ToJsonProperty("threadId");
            entity.Property(e => e.UserId).ToJsonProperty("userId");
            entity.Property(e => e.CreatedAt).ToJsonProperty("createdAt");
            entity.Property(e => e.UpdatedAt).ToJsonProperty("updatedAt");
        });

        // User-patterns container configuration
        modelBuilder.Entity<UserPattern>(entity =>
        {
            entity.ToContainer("user-patterns");
            entity.HasPartitionKey(e => e.UserId); // ✅ Critical: partition by userId
            entity.HasKey(e => e.Id);
            
            // Property mappings to camelCase JSON
            entity.Property(e => e.Id).ToJsonProperty("id");
            entity.Property(e => e.UserId).ToJsonProperty("userId");
            entity.Property(e => e.PatternType).ToJsonProperty("patternType");
            entity.Property(e => e.PatternData).ToJsonProperty("patternData");
            entity.Property(e => e.UpdatedAt).ToJsonProperty("updatedAt");
        });
    }
}

/// <summary>
/// Conversation thread entity - partition key: threadId
/// </summary>
public class ConversationThread
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string ThreadId { get; set; } = string.Empty; // ✅ Partition key
    
    public string UserId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Message> Messages { get; set; } = new();
}

/// <summary>
/// Message within a conversation thread
/// </summary>
public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Role { get; set; } = "user"; // "user" | "assistant"
    
    public string Text { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int TokenCount { get; set; }
}

/// <summary>
/// User pattern entity - partition key: userId
/// Stores learned behavior for timesheet logging, reminder preferences, etc.
/// </summary>
public class UserPattern
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string UserId { get; set; } = string.Empty; // ✅ Partition key
    
    public string PatternType { get; set; } = string.Empty; // "timesheet" | "reminder" | "approval"
    
    public string PatternData { get; set; } = "{}"; // JSON blob with pattern details
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
