using System.Text.Json.Serialization;

namespace HRAgent.Api.Models;

/// <summary>
/// Employee response from Factorial HR API.
/// Placeholder model - will be expanded in Epic 3.
/// </summary>
public class EmployeeResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("role")]
    public string? Role { get; set; }
    
    [JsonPropertyName("manager_id")]
    public string? ManagerId { get; set; }
}

/// <summary>
/// PTO balance response from Factorial HR API.
/// Placeholder model - will be expanded in Epic 3.
/// </summary>
public class PTOBalanceResponse
{
    [JsonPropertyName("total_days")]
    public int TotalDays { get; set; }
    
    [JsonPropertyName("used_days")]
    public int UsedDays { get; set; }
    
    [JsonPropertyName("available_days")]
    public int AvailableDays { get; set; }
    
    [JsonPropertyName("pending_days")]
    public int PendingDays { get; set; }
}
