using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System.Text;
using System.Text.Json;

namespace HRAgent.Api.Services;

/// <summary>
/// Thread-safe audit logging service using Azure Blob Storage append blobs.
/// Generates immutable audit trail for compliance and legal protection.
/// </summary>
public class AuditLogger
{
    private readonly BlobContainerClient _containerClient;
    private readonly SemaphoreSlim _writeLock = new(1, 1); // ✅ Thread-safe writes
    private readonly ILogger<AuditLogger> _logger;
    private const int MaxRetries = 3;

    public AuditLogger(IConfiguration configuration, ILogger<AuditLogger> logger)
    {
        _logger = logger;
        
        // Connection string injected by Aspire via WithReference(storage)
        // Fallback to manual "BlobStorage:ConnectionString" for standalone runs
        var connectionString = configuration.GetConnectionString("blobs")
            ?? configuration["BlobStorage:ConnectionString"]
            ?? throw new InvalidOperationException("Blob Storage connection string not configured");
        
        var containerName = configuration["BlobStorage:ContainerName"] ?? "audit-logs";
        
        var serviceClient = new BlobServiceClient(connectionString);
        _containerClient = serviceClient.GetBlobContainerClient(containerName);
        
        // Create container if it doesn't exist (local dev only)
        // Production container should be pre-created with immutability policy
        _containerClient.CreateIfNotExists();
    }

    /// <summary>
    /// Logs an audit event to append blob with full context.
    /// Thread-safe and includes automatic retry with exponential backoff.
    /// </summary>
    /// <param name="eventType">Type of event (e.g., "pto.requested", "timesheet.submitted")</param>
    /// <param name="userId">User ID from JWT token (required)</param>
    /// <param name="data">Event-specific data object</param>
    /// <param name="reasoning">AI reasoning or human decision rationale (optional)</param>
    /// <param name="threadId">Conversation thread ID for context (optional)</param>
    /// <param name="correlationId">Request correlation ID for distributed tracing (optional)</param>
    public async Task LogAsync(
        string eventType,
        string userId,
        object data,
        string? reasoning = null,
        string? threadId = null,
        string? correlationId = null)
    {
        if (string.IsNullOrEmpty(eventType))
            throw new ArgumentNullException(nameof(eventType));
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        var auditEntry = new
        {
            timestamp = DateTime.UtcNow, // ✅ Always UTC for compliance
            userId,
            eventType,
            data,
            reasoning,
            threadId,
            correlationId,
            version = "1.0" // Schema version for future compatibility
        };

        var jsonLine = JsonSerializer.Serialize(auditEntry, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false // Single line for JSONL format
        }) + "\n"; // ✅ Newline-separated JSON Lines format

        // Determine blob path: audit/{year}/{month}/{day}/{threadId}.jsonl
        var now = DateTime.UtcNow;
        var blobPath = $"audit/{now:yyyy}/{now:MM}/{now:dd}/{threadId ?? "system"}.jsonl";

        var retryCount = 0;
        var delay = TimeSpan.FromMilliseconds(100);

        while (retryCount < MaxRetries)
        {
            try
            {
                // ✅ Thread-safe writes using SemaphoreSlim
                await _writeLock.WaitAsync();
                try
                {
                    var appendBlobClient = _containerClient.GetAppendBlobClient(blobPath);
                    
                    // Create append blob if it doesn't exist
                    await appendBlobClient.CreateIfNotExistsAsync();
                    
                    // Append log entry as UTF-8 bytes
                    var bytes = Encoding.UTF8.GetBytes(jsonLine);
                    await appendBlobClient.AppendBlockAsync(new MemoryStream(bytes));
                    
                    _logger.LogDebug("Audit log written: {EventType} for user {UserId}", eventType, userId);
                    return; // Success
                }
                finally
                {
                    _writeLock.Release();
                }
            }
            catch (Azure.RequestFailedException ex) when (retryCount < MaxRetries - 1 && (ex.Status == 409 || ex.Status == 503))
            {
                // Retry transient Azure failures: 409 Conflict (concurrent writes), 503 Service Unavailable (throttling)
                retryCount++;
                var retryDelay = ex.Status == 503 ? TimeSpan.FromSeconds(2) : delay; // Longer delay for throttling
                _logger.LogWarning(ex, "Azure Blob Storage transient error (Status {Status}, attempt {Attempt}/{MaxRetries}). Retrying after {Delay}ms...", 
                    ex.Status, retryCount, MaxRetries, retryDelay.TotalMilliseconds);
                await Task.Delay(retryDelay);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // Exponential backoff
            }
            catch (Exception ex) when (retryCount < MaxRetries - 1)
            {
                // Retry other exceptions with exponential backoff
                retryCount++;
                _logger.LogWarning(ex, "Audit log write failed (attempt {Attempt}/{MaxRetries}). Retrying...", 
                    retryCount, MaxRetries);
                await Task.Delay(delay);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // Exponential backoff
            }
            catch (Exception ex)
            {
                // All retries exhausted - critical error
                _logger.LogError(ex, "CRITICAL: Audit log write failed after {MaxRetries} attempts. Event type: {EventType}, User: {UserId}",
                    MaxRetries, eventType, userId);
                
                // TODO: Send alert to operations team (Application Insights custom event)
                throw; // Fail operation if audit logging fails (compliance requirement)
            }
        }
    }

    /// <summary>
    /// Queries audit logs for a specific thread (used by admin dashboard).
    /// Returns all audit entries for the given threadId.
    /// </summary>
    public async Task<List<string>> QueryLogsAsync(string threadId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var logs = new List<string>();
        var start = startDate ?? DateTime.UtcNow.AddDays(-7); // Default: last 7 days
        var end = endDate ?? DateTime.UtcNow;

        // Iterate through date range (blob path structure: audit/{year}/{month}/{day}/{threadId}.jsonl)
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            var blobPath = $"audit/{date:yyyy}/{date:MM}/{date:dd}/{threadId}.jsonl";
            var appendBlobClient = _containerClient.GetAppendBlobClient(blobPath);

            if (await appendBlobClient.ExistsAsync())
            {
                var download = await appendBlobClient.DownloadContentAsync();
                var content = download.Value.Content.ToString();
                logs.Add(content); // Entire file content (JSON Lines format)
            }
        }

        return logs;
    }
}
