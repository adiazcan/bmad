using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Moq;
using HRAgent.Api.Services;

namespace HRAgent.Api.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing IMongoClient registration
            services.RemoveAll<IMongoClient>();
            
            // Remove services that depend on MongoDB
            services.RemoveAll<MongoDbService>();
            services.RemoveAll<ConversationRepository>();
            services.RemoveAll<PatternRepository>();
            
            // Add a mock IMongoClient
            var mockMongoClient = new Mock<IMongoClient>();
            services.AddSingleton(mockMongoClient.Object);
            
            // Add mock services
            services.AddScoped(_ =>
            {
                var mockMongoDb = new Mock<IMongoDatabase>();
                var mockService = new Mock<MongoDbService>(mockMongoClient.Object, "test-db");
                mockService.Setup(s => s.Database).Returns(mockMongoDb.Object);
                return mockService.Object;
            });
            
            services.AddScoped(_ => Mock.Of<ConversationRepository>());
            services.AddScoped(_ => Mock.Of<PatternRepository>());
            
            // Override configuration to provide mock connection strings
            builder.UseSetting("ConnectionStrings:blobs", "UseDevelopmentStorage=true");
            builder.UseSetting("ConnectionStrings:hragent", "mongodb://localhost:27017/test");
        });
        
        builder.UseEnvironment("Testing");
    }
}
