namespace ManiTheDev.Models;

/// <summary>
/// Generic result object for tool operations
/// </summary>
/// <typeparam name="T">Type of data returned by the tool</typeparam>
public class ToolResult<T>
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
    public T? Data { get; set; }

    /// <summary>
    /// Error message (if operation failed)
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static ToolResult<T> CreateSuccess(T data, string message = "Operation completed successfully")
    {
        return new ToolResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static ToolResult<T> CreateFailure(string error, string message = "Operation failed")
    {
        return new ToolResult<T>
        {
            Success = false,
            Message = message,
            Error = error
        };
    }
} 