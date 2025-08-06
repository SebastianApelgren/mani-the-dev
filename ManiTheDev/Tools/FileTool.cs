using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Models;
using System.Text;

namespace ManiTheDev.Tools;

public class FileTool : IFileTool
{
    private readonly string _baseDirectory;

    public FileTool(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
    }

    public async Task<ToolResult<string>> ReadFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (!File.Exists(fullPath))
            {
                return ToolResult<string>.CreateFailure($"File not found: {filePath}", "Failed to read file");
            }

            var content = await File.ReadAllTextAsync(fullPath);
            return ToolResult<string>.CreateSuccess(content, $"Successfully read file: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.CreateFailure($"Error reading file: {ex.Message}", "Failed to read file");
        }
    }

    public async Task<ToolResult<string>> WriteFileAsync(string filePath, string content)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(fullPath, content);
            return ToolResult<string>.CreateSuccess(content, $"Successfully wrote file: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.CreateFailure($"Error writing file: {ex.Message}", "Failed to write file");
        }
    }

    public async Task<ToolResult<string>> CreateFileAsync(string filePath, string content)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (File.Exists(fullPath))
            {
                return ToolResult<string>.CreateFailure($"File already exists: {filePath}", "Failed to create file");
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(fullPath, content);
            return ToolResult<string>.CreateSuccess(content, $"Successfully created file: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.CreateFailure($"Error creating file: {ex.Message}", "Failed to create file");
        }
    }

    public async Task<ToolResult<string>> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (!File.Exists(fullPath))
            {
                return ToolResult<string>.CreateFailure($"File not found: {filePath}", "Failed to delete file");
            }

            File.Delete(fullPath);
            return ToolResult<string>.CreateSuccess(string.Empty, $"Successfully deleted file: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.CreateFailure($"Error deleting file: {ex.Message}", "Failed to delete file");
        }
    }

    public async Task<ToolResult<IEnumerable<string>>> ListFilesAsync(string directoryPath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, directoryPath);
            
            if (!Directory.Exists(fullPath))
            {
                return ToolResult<IEnumerable<string>>.CreateSuccess(Enumerable.Empty<string>(), $"Directory not found: {directoryPath}");
            }

            var files = Directory.GetFiles(fullPath).Select(Path.GetFileName);
            return ToolResult<IEnumerable<string>>.CreateSuccess(files, $"Successfully listed files in directory: {directoryPath}");
        }
        catch (Exception ex)
        {
            return ToolResult<IEnumerable<string>>.CreateFailure($"Error listing files: {ex.Message}", "Failed to list files");
        }
    }

    public async Task<ToolResult<string>> AddLineAsync(string filePath, int lineNumber, string content)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (!File.Exists(fullPath))
            {
                return ToolResult<string>.CreateFailure($"File not found: {filePath}", "Failed to modify file");
            }

            if (lineNumber < 1)
            {
                return ToolResult<string>.CreateFailure("Line number must be 1 or greater", "Failed to modify file");
            }

            var lines = await File.ReadAllLinesAsync(fullPath);
            var newLines = new List<string>(lines);
            
            // Insert the new line (lineNumber is 1-based, so subtract 1 for 0-based index)
            newLines.Insert(lineNumber - 1, content);
            
            await File.WriteAllLinesAsync(fullPath, newLines);
            
            var modifiedContent = string.Join(Environment.NewLine, newLines);
            return ToolResult<string>.CreateSuccess(modifiedContent, $"Successfully added line {lineNumber} to file: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.CreateFailure($"Error modifying file: {ex.Message}", "Failed to modify file");
        }
    }

    public async Task<ToolResult<IEnumerable<int>>> SearchInFileAsync(string filePath, string searchText)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            
            if (!File.Exists(fullPath))
            {
                return ToolResult<IEnumerable<int>>.CreateFailure($"File not found: {filePath}", "Failed to search file");
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
            
            return ToolResult<IEnumerable<int>>.CreateSuccess(matchingLines, $"Found {matchingLines.Count} matching lines in file: {filePath}");
        }
        catch (Exception ex)
        {
            return ToolResult<IEnumerable<int>>.CreateFailure($"Error searching file: {ex.Message}", "Failed to search file");
        }
    }
} 