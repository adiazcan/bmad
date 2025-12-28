using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using HRAgent.Api.Models;

namespace HRAgent.Api.Clients;

/// <summary>
/// HTTP client for Factorial HR API with Polly resilience policies.
/// </summary>
public class FactorialClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FactorialClient> _logger;
    private readonly FactorialOptions _options;

    public FactorialClient(
        HttpClient httpClient,
        IOptions<FactorialOptions> options,
        ILogger<FactorialClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        
        // Configure base address and default headers
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// Retrieves employee details from Factorial HR API.
    /// Placeholder for future Epic 3 (PTO Management) integration.
    /// </summary>
    public async Task<EmployeeResponse?> GetEmployeeAsync(
        string employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Fetching employee {EmployeeId} from Factorial", employeeId);
            
            var response = await _httpClient.GetFromJsonAsync<EmployeeResponse>(
                $"/api/{_options.ApiVersion}/resources/employees/employees/{employeeId}",
                cancellationToken);
            
            _logger.LogDebug("Successfully retrieved employee {EmployeeId}", employeeId);
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching employee {EmployeeId}", employeeId);
            throw;  // Polly will handle retries
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken != cancellationToken)
        {
            _logger.LogError(ex, "Timeout fetching employee {EmployeeId}", employeeId);
            throw;  // Polly timeout policy triggered
        }
    }

    /// <summary>
    /// Retrieves PTO balance for an employee from Factorial HR API.
    /// Placeholder for future Epic 3 (PTO Management) integration.
    /// </summary>
    public async Task<PTOBalanceResponse?> GetPTOBalanceAsync(
        string employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Fetching PTO balance for employee {EmployeeId}", employeeId);
            
            var response = await _httpClient.GetFromJsonAsync<PTOBalanceResponse>(
                $"/api/{_options.ApiVersion}/resources/employees/employees/{employeeId}/time_off/balance",
                cancellationToken);
            
            _logger.LogDebug("Successfully retrieved PTO balance for employee {EmployeeId}", employeeId);
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching PTO balance for {EmployeeId}", employeeId);
            throw;  // Polly will handle retries
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken != cancellationToken)
        {
            _logger.LogError(ex, "Timeout fetching PTO balance for {EmployeeId}", employeeId);
            throw;  // Polly timeout policy triggered
        }
    }
}

/// <summary>
/// Configuration options for Factorial API client.
/// Bound from appsettings.json Factorial section.
/// </summary>
public class FactorialOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "2025-10-01";
}
