namespace ManiTheDev.Models
{
    /// <summary>
    /// Request object for database operations between AI agents.
    /// </summary>
    public class DatabaseOperationRequest
    {
        /// <summary>
        /// Gets or sets the request text describing what database operation to perform.
        /// </summary>
        public string Request { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional data needed for the database operation (e.g., file path, content, search terms).
        /// </summary>
        public string? Data { get; set; }
    }
}