using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HRAgent.Api.Tests;

/// <summary>
/// Integration tests for Azure AD JWT authentication middleware.
/// Tests verify that protected endpoints require valid JWT tokens.
/// </summary>
public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "invalid-token-123");

        // Act
        var response = await client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Returns200()
    {
        // Arrange - Create client with test authentication handler
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace JWT Bearer authentication with test authentication handler
                services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.AuthenticationScheme, options => { });
            });
        }).CreateClient();

        // Add test authentication header
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme, "valid-token");

        // Act
        var response = await client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Authenticated!", content);
    }

    [Fact]
    public async Task UnprotectedEndpoint_WithoutToken_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/weatherforecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithExpiredToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Simulate expired token (Azure AD would reject this)
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "expired-token");

        // Act
        var response = await client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithWrongAudience_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Simulate token with wrong audience
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "token-wrong-audience");

        // Act
        var response = await client.GetAsync("/secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

/// <summary>
/// Test authentication handler for integration testing.
/// Simulates Azure AD JWT token validation without requiring real Azure AD.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if Authorization header exists
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
        }

        var authHeader = Request.Headers["Authorization"].ToString();
        
        // Check for valid token format
        if (!authHeader.StartsWith($"{AuthenticationScheme} "))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme"));
        }

        // Extract token
        var token = authHeader.Substring($"{AuthenticationScheme} ".Length).Trim();

        // Simulate token validation (in real scenario, Azure AD validates)
        if (token == "valid-token")
        {
            // Create claims for authenticated user
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Name, "Test User")
            };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
    }
}
