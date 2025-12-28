using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HRAgent.Api.Tests;

public class WeatherForecastTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WeatherForecastTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");
        var contentType = response.Content.Headers.ContentType?.MediaType;

        // Assert
        Assert.Equal("application/json", contentType);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsFiveForecasts()
    {
        // Act
        var forecasts = await _client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Length);
    }

    [Fact]
    public async Task GetWeatherForecast_EachForecastHasValidData()
    {
        // Act
        var forecasts = await _client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert
        Assert.NotNull(forecasts);
        Assert.All(forecasts, forecast =>
        {
            Assert.True(forecast.Date > DateOnly.FromDateTime(DateTime.Now));
            Assert.InRange(forecast.TemperatureC, -20, 55);
            Assert.NotNull(forecast.Summary);
            Assert.NotEmpty(forecast.Summary);
            // TemperatureF should be calculated from TemperatureC
            var expectedF = 32 + (int)(forecast.TemperatureC / 0.5556);
            Assert.Equal(expectedF, forecast.TemperatureF);
        });
    }

    [Fact]
    public async Task GetWeatherForecast_UsesValidSummaries()
    {
        // Arrange
        var validSummaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", 
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // Act
        var forecasts = await _client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert
        Assert.NotNull(forecasts);
        Assert.All(forecasts, forecast =>
        {
            Assert.Contains(forecast.Summary, validSummaries);
        });
    }
}

// Match the record from Program.cs
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
