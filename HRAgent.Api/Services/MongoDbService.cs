using MongoDB.Driver;

namespace HRAgent.Api.Services;

/// <summary>
/// MongoDB database service providing access to IMongoDatabase
/// Scoped per request to provide database context
/// </summary>
public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "hragent";
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoDatabase Database => _database;
}
