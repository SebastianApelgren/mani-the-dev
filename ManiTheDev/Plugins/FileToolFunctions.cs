using System.ComponentModel;
using System.Text.Json;
using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Models;
using Microsoft.SemanticKernel;

namespace ManiTheDev.Plugins
{
    /// <summary>
    /// Semantic Kernel native functions that wrap FileTool operations.
    /// </summary>
    public class FileToolFunctions
    {
        private readonly IFileTool _fileTool;

        public FileToolFunctions(IFileTool fileTool)
        {
            _fileTool = fileTool;
        }

        private static string SerializeResult<T>(ToolResult<T> result)
        {
            var payload = new
            {
                success = result.Success,
                message = result.Message,
                error = result.Error,
                data = result.Data,
            };
            return JsonSerializer.Serialize(payload);
        }

        [KernelFunction, Description("Read a text file from the workspace root or a subfolder.")]
        public async Task<string> ReadFile([Description("Path to the file, relative to the workspace root.")] string filePath)
        {
            ToolResult<string> result = await _fileTool.ReadFileAsync(filePath);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Write content to a file, creating directories as needed.")]
        public async Task<string> WriteFile(
            [Description("Path to the file, relative to the workspace root.")] string filePath,
            [Description("Content to write.")] string content)
        {
            ToolResult<string> result = await _fileTool.WriteFileAsync(filePath, content);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Create a new file with content. Fails if the file exists.")]
        public async Task<string> CreateFile(
            [Description("Path to the new file, relative to the workspace root.")] string filePath,
            [Description("Initial content to write.")] string content)
        {
            ToolResult<string> result = await _fileTool.CreateFileAsync(filePath, content);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Delete a file.")]
        public async Task<string> DeleteFile([Description("Path to the file, relative to the workspace root.")] string filePath)
        {
            ToolResult<string> result = await _fileTool.DeleteFileAsync(filePath);
            return SerializeResult(result);
        }

        [KernelFunction, Description("List files in a directory.")]
        public async Task<string> ListFiles([Description("Path to the directory, relative to the workspace root.")] string directoryPath)
        {
            ToolResult<IEnumerable<string>> result = await _fileTool.ListFilesAsync(directoryPath);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Insert a line at a specific position in a file (1-based index).")]
        public async Task<string> AddLine(
            [Description("Path to the file, relative to the workspace root.")] string filePath,
            [Description("Line number to insert at (1-based). ")] int lineNumber,
            [Description("The content of the line to insert.")] string content)
        {
            ToolResult<string> result = await _fileTool.AddLineAsync(filePath, lineNumber, content);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Search for text within a file and return 1-based line numbers where it appears.")]
        public async Task<string> SearchInFile(
            [Description("Path to the file, relative to the workspace root.")] string filePath,
            [Description("Text to search for (case-insensitive).")] string searchText)
        {
            ToolResult<IEnumerable<int>> result = await _fileTool.SearchInFileAsync(filePath, searchText);
            return SerializeResult(result);
        }
    }
}
