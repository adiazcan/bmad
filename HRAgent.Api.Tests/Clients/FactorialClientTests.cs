using HRAgent.Api.Clients;
using HRAgent.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace HRAgent.Api.Tests.Clients;

public class FactorialClientTests
{
    private readonly Mock<ILogger<FactorialClient>> _mockLogger;
    private readonly FactorialOptions _options;

    public FactorialClientTests()
    {
        _mockLogger = new Mock<ILogger<FactorialClient>>();
        _options = new FactorialOptions
        {
            BaseUrl = "https://api.demo.factorial.dev",
            ApiKey = "test-api-key",
            ApiVersion = "2025-10-01"
        };
    }

    [Fact]
    public async Task GetEmployeeAsync_ReturnsEmployee_WhenApiSucceeds()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    "{\"id\":\"123\",\"first_name\":\"John\",\"last_name\":\"Doe\",\"email\":\"john@test.com\",\"role\":\"employee\"}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var factorialClient = new FactorialClient(
            httpClient,
            Options.Create(_options),
            _mockLogger.Object);

        // Act
        var employee = await factorialClient.GetEmployeeAsync("123");

        // Assert
        Assert.NotNull(employee);
        Assert.Equal("123", employee.Id);
        Assert.Equal("John", employee.FirstName);
        Assert.Equal("Doe", employee.LastName);
    }

    [Fact]
    public async Task GetEmployeeAsync_ThrowsHttpRequestException_WhenApiReturns500()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var factorialClient = new FactorialClient(
            httpClient,
            Options.Create(_options),
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => factorialClient.GetEmployeeAsync("123"));
    }

    [Fact]
    public async Task GetPTOBalanceAsync_ReturnsPTOBalance_WhenApiSucceeds()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    "{\"total_days\":20,\"used_days\":8,\"available_days\":12,\"pending_days\":0}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var factorialClient = new FactorialClient(
            httpClient,
            Options.Create(_options),
            _mockLogger.Object);

        // Act
        var balance = await factorialClient.GetPTOBalanceAsync("123");

        // Assert
        Assert.NotNull(balance);
        Assert.Equal(20, balance.TotalDays);
        Assert.Equal(8, balance.UsedDays);
        Assert.Equal(12, balance.AvailableDays);
    }

    [Fact]
    public async Task GetPTOBalanceAsync_ThrowsException_WhenApiKeyInvalid()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var factorialClient = new FactorialClient(
            httpClient,
            Options.Create(_options),
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => factorialClient.GetPTOBalanceAsync("123"));
    }

    [Fact]
    public async Task GetEmployeeAsync_PropagatesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var factorialClient = new FactorialClient(
            httpClient,
            Options.Create(_options),
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => factorialClient.GetEmployeeAsync("123", cts.Token));
    }
}
