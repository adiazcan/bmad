using MongoDB.Driver;
using HRAgent.Api.Data;

namespace HRAgent.Api.Services;

/// <summary>
/// Repository for conversation thread persistence and retrieval
/// </summary>
public class ConversationRepository
{
    private readonly IMongoCollection<ConversationThread> _collection;

    public ConversationRepository(MongoDbService mongoService)
    {
        _collection = mongoService.Database.GetCollection<ConversationThread>("conversations");
    }

    /// <summary>
    /// Retrieves a conversation thread by threadId (uses index)
    /// </summary>
    /// <param name="threadId">Application thread identifier</param>
    /// <returns>Conversation thread or null if not found</returns>
    public async Task<ConversationThread?> GetByThreadIdAsync(string threadId)
    {
        return await _collection
            .Find(c => c.ThreadId == threadId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all conversation threads for a user
    /// </summary>
    /// <param name="userId">User ID from Azure AD token claims</param>
    /// <returns>List of conversation threads</returns>
    public async Task<List<ConversationThread>> GetByUserIdAsync(string userId)
    {
        return await _collection
            .Find(c => c.UserId == userId)
            .SortByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Inserts a new conversation thread
    /// </summary>
    /// <param name="thread">Conversation thread to insert</param>
    public async Task AddAsync(ConversationThread thread)
    {
        await _collection.InsertOneAsync(thread);
    }

    /// <summary>
    /// Updates an existing conversation thread or inserts if not found
    /// </summary>
    /// <param name="thread">Conversation thread to update/insert</param>
    public async Task UpdateAsync(ConversationThread thread)
    {
        var filter = Builders<ConversationThread>.Filter.Eq(c => c.ThreadId, thread.ThreadId);
        await _collection.ReplaceOneAsync(filter, thread, new ReplaceOptions { IsUpsert = true });
    }

    /// <summary>
    /// Deletes a conversation thread by threadId
    /// </summary>
    /// <param name="threadId">Application thread identifier</param>
    public async Task DeleteAsync(string threadId)
    {
        await _collection.DeleteOneAsync(c => c.ThreadId == threadId);
    }
}
