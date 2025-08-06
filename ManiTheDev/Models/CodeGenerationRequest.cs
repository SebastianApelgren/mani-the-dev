namespace ManiTheDev.Models
{
    /// <summary>
    /// Request object for code generation operations between AI agents.
    /// </summary>
    public class CodeGenerationRequest
    {
        /// <summary>
        /// Gets or sets the request text describing what code to generate.
        /// </summary>
        public string Request { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional data needed for the code generation (e.g., database schema, parameters).
        /// </summary>
        public string? Data { get; set; }
    }
}