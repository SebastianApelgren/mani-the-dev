using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Models;
using ManiTheDev.Utilities;
using System.Text.Json;

namespace ManiTheDev.Tools;

public class DatabaseTool : IDatabaseTool
{
    private readonly string _baseDirectory;

    public DatabaseTool(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
    }

    public async Task<ToolResult<string>> ReadDatabaseAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (!File.Exists(fullPath))
            {
                return ToolResult<string>.CreateFailure($"Database file not found: {filePath}", "Failed to read database");
            }

            var content = await File.ReadAllTextAsync(fullPath);
            return ToolResult<string>.CreateSuccess(content, $"Successfully read database: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.CreateFailure($"Error reading database: {ex.Message}", "Failed to read database");
        }
    }

    public async Task<ToolResult<string>> WriteDatabaseAsync(string filePath, string jsonContent)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            // Validate JSON before writing
            if (!JsonUtility.ValidateJson(jsonContent))
            {
                return ToolResult<string>.CreateFailure("Invalid JSON content provided", "Failed to write database");
            }
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
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

    public async Task<ToolResult<string>> CreateDatabaseAsync(string filePath, string jsonContent)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
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
            var directory = Path.GetDirectoryName(fullPath);
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

    public async Task<ToolResult<string>> AddLineAsync(string filePath, int lineNumber, string content, bool replace = false)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (!File.Exists(fullPath))
            {
                return ToolResult<string>.CreateFailure($"Database file not found: {filePath}", "Failed to modify database");
            }

            if (lineNumber < 1)
            {
                return ToolResult<string>.CreateFailure("Line number must be 1 or greater", "Failed to modify database");
            }

            // Read current content
            var lines = await File.ReadAllLinesAsync(fullPath);
            var newLines = new List<string>(lines);
            
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
            var modifiedContent = string.Join(Environment.NewLine, newLines);
            if (!JsonUtility.ValidateJson(modifiedContent))
            {
                return ToolResult<string>.CreateFailure("The modification would create invalid JSON. Please check the content and line position.", "Failed to modify database");
            }

            // Write the validated content
            await File.WriteAllLinesAsync(fullPath, newLines);
            
            var action = replace ? "replaced" : "added";
            return ToolResult<string>.CreateSuccess(modifiedContent, $"Successfully {action} line {lineNumber} in database: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.CreateFailure($"Error modifying database: {ex.Message}", "Failed to modify database");
        }
    }

    public async Task<ToolResult<IEnumerable<int>>> SearchInDatabaseAsync(string filePath, string searchText)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (!File.Exists(fullPath))
            {
                return ToolResult<IEnumerable<int>>.CreateFailure($"Database file not found: {filePath}", "Failed to search database");
            }

            var lines = await File.ReadAllLinesAsync(fullPath);
            var matchingLines = new List<int>();
            
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