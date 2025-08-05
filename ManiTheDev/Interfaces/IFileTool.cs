namespace ManiTheDev.Interfaces;

public interface IFileTool
{
    /// <summary>
    /// Reads the content of a file
    /// </summary>
    /// <param name="filePath">Path to the file (relative to agent folder)</param>
    /// <returns>File content as string</returns>
    Task<string> ReadFileAsync(string filePath);
    
    /// <summary>
    /// Writes content to a file, overwriting if it exists
    /// </summary>
    /// <param name="filePath">Path to the file (relative to agent folder)</param>
    /// <param name="content">Content to write</param>
    Task WriteFileAsync(string filePath, string content);
    
    /// <summary>
    /// Creates a new file with the specified content
    /// </summary>
    /// <param name="filePath">Path to the file (relative to agent folder)</param>
    /// <param name="content">Content to write</param>
    Task CreateFileAsync(string filePath, string content);
    
    /// <summary>
    /// Deletes a file
    /// </summary>
    /// <param name="filePath">Path to the file (relative to agent folder)</param>
    Task DeleteFileAsync(string filePath);
    
    /// <summary>
    /// Lists files in a directory
    /// </summary>
    /// <param name="directoryPath">Path to the directory (relative to agent folder)</param>
    /// <returns>List of file names</returns>
    Task<IEnumerable<string>> ListFilesAsync(string directoryPath);
    
    /// <summary>
    /// Adds a line to a specific position in a file
    /// </summary>
    /// <param name="filePath">Path to the file (relative to agent folder)</param>
    /// <param name="lineNumber">Line number to insert at (1-based)</param>
    /// <param name="content">Content to insert</param>
    Task AddLineAsync(string filePath, int lineNumber, string content);
    
    /// <summary>
    /// Searches for text within a file
    /// </summary>
    /// <param name="filePath">Path to the file (relative to agent folder)</param>
    /// <param name="searchText">Text to search for</param>
    /// <returns>List of line numbers containing the search text</returns>
    Task<IEnumerable<int>> SearchInFileAsync(string filePath, string searchText);
} 