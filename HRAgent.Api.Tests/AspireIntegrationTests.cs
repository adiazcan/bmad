using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HRAgent.Api.Tests;

/// <summary>
/// Integration tests to verify Aspire orchestration configuration
/// </summary>
public class AspireIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AspireIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task WeatherForecast_ReturnsSuccess_WhenCalled()
    {
        // Arrange - no setup needed

        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WeatherForecast_ReturnsJsonContent()
    {
        // Arrange - no setup needed

        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotEmpty(content);
        Assert.Contains("temperatureC", content);
        Assert.Contains("summary", content);
    }

    [Fact]
    public async Task WeatherForecast_ReturnsArrayOfData()
    {
        // Arrange - no setup needed

        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var weatherForecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();

        // Assert
        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);
        Assert.All(weatherForecasts, forecast =>
        {
            Assert.NotEqual(default, forecast.Date);
            Assert.InRange(forecast.TemperatureC, -50, 50); // Reasonable temperature range
        });
    }
}

/// <summary>
/// Custom web application factory that disables MongoDB initialization for tests
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set testing environment to skip MongoDB initialization
        builder.UseEnvironment("Testing");
        
        // Provide required configuration values for testing
        builder.UseSetting("ConnectionStrings:blobs", "UseDevelopmentStorage=true");
        builder.UseSetting("ConnectionStrings:hragent", "mongodb://localhost:27017/test");
        
        base.ConfigureWebHost(builder);
    }
}
