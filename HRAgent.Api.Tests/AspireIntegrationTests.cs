using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HRAgent.Api.Tests;

/// <summary>
/// Integration tests to verify Aspire orchestration configuration
/// </summary>
public class AspireIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AspireIntegrationTests(WebApplicationFactory<Program> factory)
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
