using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HRAgent.Api.Data;

/// <summary>
/// Conversation thread document - indexed by threadId
/// Stores chat conversations with message history
/// </summary>
public class ConversationThread
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("threadId")]
    public string ThreadId { get; set; } = string.Empty;
    
    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;
    
    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("messages")]
    public List<Message> Messages { get; set; } = new();
}

/// <summary>
/// Message within a conversation thread
/// </summary>
public class Message
{
    [BsonElement("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [BsonElement("role")]
    public string Role { get; set; } = "user"; // "user" | "assistant"
    
    [BsonElement("text")]
    public string Text { get; set; } = string.Empty;
    
    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("tokenCount")]
    public int TokenCount { get; set; }
}

/// <summary>
/// User pattern document - indexed by userId
/// Stores learned behavior for timesheet logging, reminder preferences, etc.
/// </summary>
public class UserPattern
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;
    
    [BsonElement("patternType")]
    public string PatternType { get; set; } = string.Empty; // "timesheet" | "reminder" | "approval"
    
    [BsonElement("patternData")]
    public string PatternData { get; set; } = "{}"; // JSON blob with pattern details
    
    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
