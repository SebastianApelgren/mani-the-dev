using ManiTheDev.Models;

namespace ManiTheDev.Interfaces;

public interface IDatabaseTool
{
    /// <summary>
    /// Reads the JSON database file
    /// </summary>
    /// <param name="filePath">Path to the JSON file (relative to agent folder)</param>
    /// <returns>JSON content as string</returns>
    Task<ToolResult<string>> ReadDatabaseAsync(string filePath);
    
    /// <summary>
    /// Writes content to the JSON database file
    /// </summary>
    /// <param name="filePath">Path to the JSON file (relative to agent folder)</param>
    /// <param name="jsonContent">JSON content to write</param>
    Task<ToolResult<string>> WriteDatabaseAsync(string filePath, string jsonContent);
    
    /// <summary>
    /// Creates a new JSON database file with the specified content
    /// </summary>
    /// <param name="filePath">Path to the JSON file (relative to agent folder)</param>
    /// <param name="jsonContent">Initial JSON content</param>
    Task<ToolResult<string>> CreateDatabaseAsync(string filePath, string jsonContent);
    
    /// <summary>
    /// Adds or replaces a line at a specific position in the JSON database file
    /// </summary>
    /// <param name="filePath">Path to the JSON file (relative to agent folder)</param>
    /// <param name="lineNumber">Line number to insert/replace at (1-based)</param>
    /// <param name="content">Content to insert/replace</param>
    /// <param name="replace">If true, replaces the line; if false, inserts a new line</param>
    Task<ToolResult<string>> AddLineAsync(string filePath, int lineNumber, string content, bool replace = false);
    
    /// <summary>
    /// Searches for text within the JSON database file
    /// </summary>
    /// <param name="filePath">Path to the JSON file (relative to agent folder)</param>
    /// <param name="searchText">Text to search for</param>
    /// <returns>List of line numbers containing the search text</returns>
    Task<ToolResult<IEnumerable<int>>> SearchInDatabaseAsync(string filePath, string searchText);
} 