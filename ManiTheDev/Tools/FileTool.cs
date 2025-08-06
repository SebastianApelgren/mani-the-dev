using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Models;

namespace ManiTheDev.Tools
{
    /// <summary>
    /// Implementation of file system operations.
    /// </summary>
    public class FileTool : IFileTool
    {
        private readonly string _baseDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTool"/> class.
        /// </summary>
        /// <param name="baseDirectory">The base directory for file operations.</param>
        public FileTool(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        /// <summary>
        /// Reads the content of a file.
        /// </summary>
        /// <param name="filePath">Path to the file (relative to agent folder).</param>
        /// <returns>ToolResult containing file content as string.</returns>
        public async Task<ToolResult<string>> ReadFileAsync(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (!File.Exists(fullPath))
                {
                    return ToolResult<string>.CreateFailure($"File not found: {filePath}", "Failed to read file");
                }

                string content = await File.ReadAllTextAsync(fullPath);
                return ToolResult<string>.CreateSuccess(content, $"Successfully read file: {filePath}");
            }
            catch (Exception ex)
            {
                return ToolResult<string>.CreateFailure($"Error reading file: {ex.Message}", "Failed to read file");
            }
        }

        /// <summary>
        /// Writes content to a file, overwriting if it exists.
        /// </summary>
        /// <param name="filePath">Path to the file (relative to agent folder).</param>
        /// <param name="content">Content to write.</param>
        /// <returns>ToolResult indicating success or failure.</returns>
        public async Task<ToolResult<string>> WriteFileAsync(string filePath, string content)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                // Ensure directory exists
                string? directory = Path.GetDirectoryName(fullPath);
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

        /// <summary>
        /// Creates a new file with the specified content.
        /// </summary>
        /// <param name="filePath">Path to the file (relative to agent folder).</param>
        /// <param name="content">Content to write.</param>
        /// <returns>ToolResult indicating success or failure.</returns>
        public async Task<ToolResult<string>> CreateFileAsync(string filePath, string content)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (File.Exists(fullPath))
                {
                    return ToolResult<string>.CreateFailure($"File already exists: {filePath}", "Failed to create file");
                }

                // Ensure directory exists
                string? directory = Path.GetDirectoryName(fullPath);
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

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="filePath">Path to the file (relative to agent folder).</param>
        /// <returns>ToolResult indicating success or failure.</returns>
        public async Task<ToolResult<string>> DeleteFileAsync(string filePath)
        {
            // Ensure method is async
            await Task.CompletedTask;

            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

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

        /// <summary>
        /// Lists files in a directory.
        /// </summary>
        /// <param name="directoryPath">Path to the directory (relative to agent folder).</param>
        /// <returns>ToolResult containing list of file names.</returns>
        public async Task<ToolResult<IEnumerable<string>>> ListFilesAsync(string directoryPath)
        {
            // Ensure method is async
            await Task.CompletedTask;

            try
            {
                string fullPath = Path.Combine(_baseDirectory, directoryPath);

                if (!Directory.Exists(fullPath))
                {
                    return ToolResult<IEnumerable<string>>.CreateSuccess(Enumerable.Empty<string>(), $"Directory not found: {directoryPath}");
                }

                IEnumerable<string?> files = Directory.GetFiles(fullPath).Select(Path.GetFileName);

                List<string> fileList = new List<string>();

                foreach (string? file in files)
                {
                    // Ensure file name is not null
                    if (file != null)
                    {
                        fileList.Add(file);
                    }
                }

                return ToolResult<IEnumerable<string>>.CreateSuccess(fileList, $"Successfully listed files in directory: {directoryPath}");
            }
            catch (Exception ex)
            {
                return ToolResult<IEnumerable<string>>.CreateFailure($"Error listing files: {ex.Message}", "Failed to list files");
            }
        }

        /// <summary>
        /// Adds a line to a specific position in a file.
        /// </summary>
        /// <param name="filePath">Path to the file (relative to agent folder).</param>
        /// <param name="lineNumber">Line number to insert at (1-based).</param>
        /// <param name="content">Content to insert.</param>
        /// <returns>ToolResult indicating success or failure.</returns>
        public async Task<ToolResult<string>> AddLineAsync(string filePath, int lineNumber, string content)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (!File.Exists(fullPath))
                {
                    return ToolResult<string>.CreateFailure($"File not found: {filePath}", "Failed to modify file");
                }

                if (lineNumber < 1)
                {
                    return ToolResult<string>.CreateFailure("Line number must be 1 or greater", "Failed to modify file");
                }

                string[] lines = await File.ReadAllLinesAsync(fullPath);
                List<string> newLines = new List<string>(lines);

                // Insert the new line (lineNumber is 1-based, so subtract 1 for 0-based index)
                newLines.Insert(lineNumber - 1, content);

                await File.WriteAllLinesAsync(fullPath, newLines);

                string modifiedContent = string.Join(Environment.NewLine, newLines);
                return ToolResult<string>.CreateSuccess(modifiedContent, $"Successfully added line {lineNumber} to file: {filePath}");
            }
            catch (Exception ex)
            {
                return ToolResult<string>.CreateFailure($"Error modifying file: {ex.Message}", "Failed to modify file");
            }
        }

        /// <summary>
        /// Searches for text within a file.
        /// </summary>
        /// <param name="filePath">Path to the file (relative to agent folder).</param>
        /// <param name="searchText">Text to search for.</param>
        /// <returns>ToolResult containing list of line numbers containing the search text.</returns>
        public async Task<ToolResult<IEnumerable<int>>> SearchInFileAsync(string filePath, string searchText)
        {
            try
            {
                string fullPath = Path.Combine(_baseDirectory, filePath);

                if (!File.Exists(fullPath))
                {
                    return ToolResult<IEnumerable<int>>.CreateFailure($"File not found: {filePath}", "Failed to search file");
                }

                string[] lines = await File.ReadAllLinesAsync(fullPath);
                List<int> matchingLines = new List<int>();

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        // Convert to 1-based line numbers
                        matchingLines.Add(i + 1);
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
}