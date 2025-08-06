namespace ManiTheDev.Models;

/// <summary>
/// Response object for agent operations
/// </summary>
public class AgentResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Human-readable message about the operation
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Data returned by the operation (if successful)
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// Error message (if operation failed)
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static AgentResponse CreateSuccess(string data, string message = "Operation completed successfully")
    {
        return new AgentResponse
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed response
    /// </summary>
    public static AgentResponse CreateFailure(string error, string message = "Operation failed")
    {
        return new AgentResponse
        {
            Success = false,
            Message = message,
            Error = error
        };
    }
} 