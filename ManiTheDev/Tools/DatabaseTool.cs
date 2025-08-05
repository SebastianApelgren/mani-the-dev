using ManiTheDev.Interfaces;
using ManiTheDev.Models;
using ManiTheDev.Utilities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ManiTheDev.Tools;

public class DatabaseTool : IDatabaseTool
{
    private readonly ILogger<DatabaseTool> _logger;
    private readonly string _baseDirectory;

    public DatabaseTool(ILogger<DatabaseTool> logger, string baseDirectory)
    {
        _logger = logger;
        _baseDirectory = baseDirectory;
    }

    public async Task<ToolResult<string>> ReadDatabaseAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Reading database: {FilePath}", fullPath);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Database file does not exist: {FilePath}", fullPath);
                return ToolResult<string>.CreateFailure($"Database file not found: {filePath}", "Failed to read database");
            }

            var content = await File.ReadAllTextAsync(fullPath);
            _logger.LogInformation("Successfully read database: {FilePath}, Size: {Size} bytes", fullPath, content.Length);
            return ToolResult<string>.CreateSuccess(content, $"Successfully read database: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading database: {FilePath}", filePath);
            return ToolResult<string>.CreateFailure($"Error reading database: {ex.Message}", "Failed to read database");
        }
    }

    public async Task<ToolResult<string>> WriteDatabaseAsync(string filePath, string jsonContent)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Writing database: {FilePath}, Size: {Size} bytes", fullPath, jsonContent.Length);
            
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
                _logger.LogInformation("Created directory: {Directory}", directory);
            }

            await File.WriteAllTextAsync(fullPath, jsonContent);
            _logger.LogInformation("Successfully wrote database: {FilePath}", fullPath);
            return ToolResult<string>.CreateSuccess(jsonContent, $"Successfully wrote database: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing database: {FilePath}", filePath);
            return ToolResult<string>.CreateFailure($"Error writing database: {ex.Message}", "Failed to write database");
        }
    }

    public async Task<ToolResult<string>> CreateDatabaseAsync(string filePath, string jsonContent)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Creating database: {FilePath}, Size: {Size} bytes", fullPath, jsonContent.Length);
            
            if (File.Exists(fullPath))
            {
                _logger.LogWarning("Database file already exists: {FilePath}", fullPath);
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
                _logger.LogInformation("Created directory: {Directory}", directory);
            }

            await File.WriteAllTextAsync(fullPath, jsonContent);
            _logger.LogInformation("Successfully created database: {FilePath}", fullPath);
            return ToolResult<string>.CreateSuccess(jsonContent, $"Successfully created database: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database: {FilePath}", filePath);
            return ToolResult<string>.CreateFailure($"Error creating database: {ex.Message}", "Failed to create database");
        }
    }

    public async Task<ToolResult<string>> AddLineAsync(string filePath, int lineNumber, string content, bool replace = false)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Adding/replacing line {LineNumber} to database: {FilePath}, Replace: {Replace}", lineNumber, fullPath, replace);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Database file does not exist: {FilePath}", fullPath);
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
            _logger.LogInformation("Successfully modified database: {FilePath}", fullPath);
            
            var action = replace ? "replaced" : "added";
            return ToolResult<string>.CreateSuccess(modifiedContent, $"Successfully {action} line {lineNumber} in database: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error modifying database: {FilePath}", filePath);
            return ToolResult<string>.CreateFailure($"Error modifying database: {ex.Message}", "Failed to modify database");
        }
    }

    public async Task<ToolResult<IEnumerable<int>>> SearchInDatabaseAsync(string filePath, string searchText)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Searching for '{SearchText}' in database: {FilePath}", searchText, fullPath);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Database file does not exist: {FilePath}", fullPath);
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
            
            _logger.LogInformation("Found {Count} matching lines in database: {FilePath}", matchingLines.Count, fullPath);
            return ToolResult<IEnumerable<int>>.CreateSuccess(matchingLines, $"Found {matchingLines.Count} matching lines in database: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching in database: {FilePath}", filePath);
            return ToolResult<IEnumerable<int>>.CreateFailure($"Error searching database: {ex.Message}", "Failed to search database");
        }
    }
} 