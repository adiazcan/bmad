using Microsoft.Extensions.Diagnostics.HealthChecks;
using HRAgent.Api.Clients;

namespace HRAgent.Api.Services;

/// <summary>
/// Health check for Factorial HR API connectivity.
/// </summary>
public class FactorialHealthCheck : IHealthCheck
{
    private readonly FactorialClient _factorialClient;
    private readonly ILogger<FactorialHealthCheck> _logger;

    public FactorialHealthCheck(
        FactorialClient factorialClient,
        ILogger<FactorialHealthCheck> logger)
    {
        _factorialClient = factorialClient;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Attempt to fetch employee (using known demo employee ID)
            // In production, this validates API key and connectivity
            var employee = await _factorialClient.GetEmployeeAsync("1779508", cancellationToken);
            
            return HealthCheckResult.Healthy("Factorial API is reachable");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Factorial API health check failed (HTTP error)");
            return HealthCheckResult.Degraded(
                "Factorial API connectivity issue - retries may succeed",
                exception: ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Factorial API health check failed (unexpected error)");
            return HealthCheckResult.Unhealthy(
                "Factorial API unavailable",
                exception: ex);
        }
    }
}
