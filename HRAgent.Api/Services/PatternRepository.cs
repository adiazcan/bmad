using MongoDB.Driver;
using HRAgent.Api.Data;

namespace HRAgent.Api.Services;

/// <summary>
/// Repository for user pattern persistence and retrieval
/// </summary>
public class PatternRepository
{
    private readonly IMongoCollection<UserPattern> _collection;

    public PatternRepository(MongoDbService mongoService)
    {
        _collection = mongoService.Database.GetCollection<UserPattern>("user-patterns");
    }

    /// <summary>
    /// Retrieves user pattern by userId (uses index)
    /// </summary>
    /// <param name="userId">User ID from Azure AD token claims</param>
    /// <returns>User pattern or null if not found</returns>
    public async Task<UserPattern?> GetByUserIdAsync(string userId)
    {
        return await _collection
            .Find(p => p.UserId == userId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Updates an existing user pattern or inserts if not found
    /// </summary>
    /// <param name="pattern">User pattern to update/insert</param>
    public async Task UpsertAsync(UserPattern pattern)
    {
        pattern.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<UserPattern>.Filter.Eq(p => p.UserId, pattern.UserId);
        await _collection.ReplaceOneAsync(filter, pattern, new ReplaceOptions { IsUpsert = true });
    }
}
