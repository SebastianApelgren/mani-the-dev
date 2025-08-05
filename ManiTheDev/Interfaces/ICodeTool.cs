namespace ManiTheDev.Interfaces;

public interface ICodeTool
{
    /// <summary>
    /// Generates code based on database schema or structure
    /// </summary>
    /// <param name="databasePath">Path to the database file</param>
    /// <param name="entityName">Name of the entity to generate code for</param>
    /// <returns>Generated code or status message</returns>
    Task<string> GenerateFromDatabaseAsync(string databasePath, string entityName);
    
    /// <summary>
    /// Creates a hook function for a specific purpose
    /// </summary>
    /// <param name="hookType">Type of hook to create</param>
    /// <param name="parameters">Parameters for the hook function</param>
    /// <returns>Generated hook function code or status message</returns>
    Task<string> CreateHookFunctionAsync(string hookType, IEnumerable<string> parameters);
    
    /// <summary>
    /// Validates generated code syntax
    /// </summary>
    /// <param name="code">Code to validate</param>
    /// <param name="language">Programming language</param>
    /// <returns>Validation result</returns>
    Task<string> ValidateCodeAsync(string code, string language);
} 