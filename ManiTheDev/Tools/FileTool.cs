using ManiTheDev.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ManiTheDev.Tools;

public class FileTool : IFileTool
{
    private readonly ILogger<FileTool> _logger;
    private readonly string _baseDirectory;

    public FileTool(ILogger<FileTool> logger, string baseDirectory)
    {
        _logger = logger;
        _baseDirectory = baseDirectory;
    }

    public async Task<string> ReadFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Reading file: {FilePath}", fullPath);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", fullPath);
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var content = await File.ReadAllTextAsync(fullPath);
            _logger.LogInformation("Successfully read file: {FilePath}, Size: {Size} bytes", fullPath, content.Length);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task WriteFileAsync(string filePath, string content)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Writing file: {FilePath}, Size: {Size} bytes", fullPath, content.Length);
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("Created directory: {Directory}", directory);
            }

            await File.WriteAllTextAsync(fullPath, content);
            _logger.LogInformation("Successfully wrote file: {FilePath}", fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task CreateFileAsync(string filePath, string content)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Creating file: {FilePath}, Size: {Size} bytes", fullPath, content.Length);
            
            if (File.Exists(fullPath))
            {
                _logger.LogWarning("File already exists: {FilePath}", fullPath);
                throw new InvalidOperationException($"File already exists: {filePath}");
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("Created directory: {Directory}", directory);
            }

            await File.WriteAllTextAsync(fullPath, content);
            _logger.LogInformation("Successfully created file: {FilePath}", fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Deleting file: {FilePath}", fullPath);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", fullPath);
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            File.Delete(fullPath);
            _logger.LogInformation("Successfully deleted file: {FilePath}", fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string directoryPath)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, directoryPath);
            _logger.LogInformation("Listing files in directory: {DirectoryPath}", fullPath);
            
            if (!Directory.Exists(fullPath))
            {
                _logger.LogWarning("Directory does not exist: {DirectoryPath}", fullPath);
                return Enumerable.Empty<string>();
            }

            var files = Directory.GetFiles(fullPath).Select(Path.GetFileName);
            _logger.LogInformation("Found {Count} files in directory: {DirectoryPath}", files.Count(), fullPath);
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files in directory: {DirectoryPath}", directoryPath);
            throw;
        }
    }

    public async Task AddLineAsync(string filePath, int lineNumber, string content)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Adding line {LineNumber} to file: {FilePath}", lineNumber, fullPath);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", fullPath);
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            if (lineNumber < 1)
            {
                throw new ArgumentException("Line number must be 1 or greater", nameof(lineNumber));
            }

            var lines = await File.ReadAllLinesAsync(fullPath);
            var newLines = new List<string>(lines);
            
            // Insert the new line (lineNumber is 1-based, so subtract 1 for 0-based index)
            newLines.Insert(lineNumber - 1, content);
            
            await File.WriteAllLinesAsync(fullPath, newLines);
            _logger.LogInformation("Successfully added line {LineNumber} to file: {FilePath}", lineNumber, fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding line {LineNumber} to file: {FilePath}", lineNumber, filePath);
            throw;
        }
    }

    public async Task<IEnumerable<int>> SearchInFileAsync(string filePath, string searchText)
    {
        try
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);
            _logger.LogInformation("Searching for '{SearchText}' in file: {FilePath}", searchText, fullPath);
            
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", fullPath);
                throw new FileNotFoundException($"File not found: {filePath}");
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
            
            _logger.LogInformation("Found {Count} matching lines in file: {FilePath}", matchingLines.Count, fullPath);
            return matchingLines;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching in file: {FilePath}", filePath);
            throw;
        }
    }
} 