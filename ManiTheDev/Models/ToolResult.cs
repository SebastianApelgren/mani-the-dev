namespace ManiTheDev.Models
{
    /// <summary>
    /// Generic result object for tool operations.
    /// </summary>
    /// <typeparam name="T">Type of data returned by the tool.</typeparam>
    public class ToolResult<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the human-readable message about the operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data returned by the operation (if successful).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the error message (if operation failed).
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="data">The data to include in the result.</param>
        /// <param name="message">The success message.</param>
        /// <returns>A successful tool result.</returns>
        public static ToolResult<T> CreateSuccess(T data, string message = "Operation completed successfully")
        {
            return new ToolResult<T>
            {
                Success = true,
                Message = message,
                Data = data,
            };
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <param name="message">The failure message.</param>
        /// <returns>A failed tool result.</returns>
        public static ToolResult<T> CreateFailure(string error, string message = "Operation failed")
        {
            return new ToolResult<T>
            {
                Success = false,
                Message = message,
                Error = error,
            };
        }
    }
}