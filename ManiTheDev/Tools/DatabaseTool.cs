using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Models;
using ManiTheDev.Utilities;

namespace ManiTheDev.Tools
{
    /// <summary>
    /// Implementation of JSON database file operations.
    /// </summary>
    public class DatabaseTool : IDatabaseTool
    {
        private readonly string _baseDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTool"/> class.
        /// </summary>
        /// <param name="baseDirectory">The base directory for database operations.</param>
        public DatabaseTool(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        /// <summary>
        /// Reads the JSON database file.
        /// </summary>
        /// <param name="filePath">Path to the JSON file (relative to agent folder).</param>
        /// <returns>JSON content as string.</returns>
        public async Task<ToolResult<string>> ReadDatabaseAsync(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (!File.Exists(fullPath))
                {
                    return ToolResult<string>.CreateFailure($"Database file not found: {filePath}", "Failed to read database");
                }

                string content = await File.ReadAllTextAsync(fullPath);
                return ToolResult<string>.CreateSuccess(content, $"Successfully read database: {filePath}");
            }
            catch (Exception ex)
            {
                return ToolResult<string>.CreateFailure($"Error reading database: {ex.Message}", "Failed to read database");
            }
        }

        /// <summary>
        /// Writes content to the JSON database file.
        /// </summary>
        /// <param name="filePath">Path to the JSON file (relative to agent folder).</param>
        /// <param name="jsonContent">JSON content to write.</param>
        /// <returns>Result of the write operation.</returns>
        public async Task<ToolResult<string>> WriteDatabaseAsync(string filePath, string jsonContent)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                // Validate JSON before writing
                if (!JsonUtility.ValidateJson(jsonContent))
                {
                    return ToolResult<string>.CreateFailure("Invalid JSON content provided", "Failed to write database");
                }

                // Ensure directory exists
                string? directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fullPath, jsonContent);
                return ToolResult<string>.CreateSuccess(jsonContent, $"Successfully wrote database: {filePath}");
            }
            catch (Exception ex)
            {
                return ToolResult<string>.CreateFailure($"Error writing database: {ex.Message}", "Failed to write database");
            }
        }

        /// <summary>
        /// Creates a new JSON database file with the specified content.
        /// </summary>
        /// <param name="filePath">Path to the JSON file (relative to agent folder).</param>
        /// <param name="jsonContent">Initial JSON content.</param>
        /// <returns>Result of the create operation.</returns>
        public async Task<ToolResult<string>> CreateDatabaseAsync(string filePath, string jsonContent)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (File.Exists(fullPath))
                {
                    return ToolResult<string>.CreateFailure($"Database file already exists: {filePath}", "Failed to create database");
                }

                // Validate JSON before creating
                if (!JsonUtility.ValidateJson(jsonContent))
                {
                    return ToolResult<string>.CreateFailure("Invalid JSON content provided", "Failed to create database");
                }

                // Ensure directory exists
                string? directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fullPath, jsonContent);
                return ToolResult<string>.CreateSuccess(jsonContent, $"Successfully created database: {filePath}");
            }
            catch (Exception ex)
            {
                return ToolResult<string>.CreateFailure($"Error creating database: {ex.Message}", "Failed to create database");
            }
        }

        /// <summary>
        /// Adds or replaces a line at a specific position in the JSON database file.
        /// </summary>
        /// <param name="filePath">Path to the JSON file (relative to agent folder).</param>
        /// <param name="lineNumber">Line number to insert/replace at (1-based).</param>
        /// <param name="content">Content to insert/replace.</param>
        /// <param name="replace">If true, replaces the line; if false, inserts a new line.</param>
        /// <returns>Result of the line modification operation.</returns>
        public async Task<ToolResult<string>> AddLineAsync(string filePath, int lineNumber, string content, bool replace = false)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (!File.Exists(fullPath))
                {
                    return ToolResult<string>.CreateFailure($"Database file not found: {filePath}", "Failed to modify database");
                }

                if (lineNumber < 1)
                {
                    return ToolResult<string>.CreateFailure("Line number must be 1 or greater", "Failed to modify database");
                }

                // Read current content
                string[] lines = await File.ReadAllLinesAsync(fullPath);
                List<string> newLines = new List<string>(lines);

                // Validate that the line number is within bounds
                if (replace && lineNumber > newLines.Count)
                {
                    return ToolResult<string>.CreateFailure($"Line {lineNumber} does not exist for replacement", "Failed to modify database");
                }

                // Insert or replace the line
                if (replace)
                {
                    newLines[lineNumber - 1] = content; // Replace existing line
                }
                else
                {
                    newLines.Insert(lineNumber - 1, content); // Insert new line
                }

                // Validate the modified JSON
                string modifiedContent = string.Join(Environment.NewLine, newLines);
                if (!JsonUtility.ValidateJson(modifiedContent))
                {
                    return ToolResult<string>.CreateFailure("The modification would create invalid JSON. Please check the content and line position.", "Failed to modify database");
                }

                // Write the validated content
                await File.WriteAllLinesAsync(fullPath, newLines);

                string action = replace ? "replaced" : "added";
                return ToolResult<string>.CreateSuccess(modifiedContent, $"Successfully {action} line {lineNumber} in database: {filePath}");
            }
            catch (Exception ex)
            {
                return ToolResult<string>.CreateFailure($"Error modifying database: {ex.Message}", "Failed to modify database");
            }
        }

        /// <summary>
        /// Searches for text within the JSON database file.
        /// </summary>
        /// <param name="filePath">Path to the JSON file (relative to agent folder).</param>
        /// <param name="searchText">Text to search for.</param>
        /// <returns>List of line numbers containing the search text.</returns>
        public async Task<ToolResult<IEnumerable<int>>> SearchInDatabaseAsync(string filePath, string searchText)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (!File.Exists(fullPath))
                {
                    return ToolResult<IEnumerable<int>>.CreateFailure($"Database file not found: {filePath}", "Failed to search database");
                }

                string[] lines = await File.ReadAllLinesAsync(fullPath);
                List<int> matchingLines = new List<int>();

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingLines.Add(i + 1); // Convert to 1-based line numbers
                    }
                }

                return ToolResult<IEnumerable<int>>.CreateSuccess(matchingLines, $"Found {matchingLines.Count} matching lines in database: {filePath}");
            }
            catch (Exception ex)
            {
                return ToolResult<IEnumerable<int>>.CreateFailure($"Error searching database: {ex.Message}", "Failed to search database");
            }
        }
    }
}