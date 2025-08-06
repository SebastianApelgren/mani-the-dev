namespace ManiTheDev.Models
{
    /// <summary>
    /// Response object for agent operations.
    /// </summary>
    public class AgentResponse
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
        public string? Data { get; set; }

        /// <summary>
        /// Gets or sets the error message (if operation failed).
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Creates a successful response.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">The success message.</param>
        /// <returns>A successful agent response.</returns>
        public static AgentResponse CreateSuccess(string data, string message = "Operation completed successfully")
        {
            return new AgentResponse
            {
                Success = true,
                Message = message,
                Data = data,
            };
        }

        /// <summary>
        /// Creates a failed response.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <param name="message">The failure message.</param>
        /// <returns>A failed agent response.</returns>
        public static AgentResponse CreateFailure(string error, string message = "Operation failed")
        {
            return new AgentResponse
            {
                Success = false,
                Message = message,
                Error = error,
            };
        }
    }
}