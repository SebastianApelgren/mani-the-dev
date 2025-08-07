using System.ComponentModel;
using System.Text.Json;
using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Models;
using Microsoft.SemanticKernel;

namespace ManiTheDev.Plugins
{
    /// <summary>
    /// Semantic Kernel native functions that wrap DatabaseTool operations.
    /// </summary>
    public class DatabaseToolFunctions
    {
        private readonly IDatabaseTool _databaseTool;

        public DatabaseToolFunctions(IDatabaseTool databaseTool)
        {
            _databaseTool = databaseTool;
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

        [KernelFunction, Description("Read a JSON database file from the database directory.")]
        public async Task<string> ReadDatabase([Description("Path to the JSON file, relative to the database directory.")] string filePath)
        {
            ToolResult<string> result = await _databaseTool.ReadDatabaseAsync(filePath);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Write JSON content to a database file (validates JSON).")]
        public async Task<string> WriteDatabase(
            [Description("Path to the JSON file, relative to the database directory.")] string filePath,
            [Description("JSON content to write.")] string jsonContent)
        {
            ToolResult<string> result = await _databaseTool.WriteDatabaseAsync(filePath, jsonContent);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Create a new JSON database file with the specified content (validates JSON).")]
        public async Task<string> CreateDatabase(
            [Description("Path to the JSON file, relative to the database directory.")] string filePath,
            [Description("Initial JSON content.")] string jsonContent)
        {
            ToolResult<string> result = await _databaseTool.CreateDatabaseAsync(filePath, jsonContent);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Add or replace a line at a specific position in the JSON database file.")]
        public async Task<string> AddLineToDatabase(
            [Description("Path to the JSON file, relative to the database directory.")] string filePath,
            [Description("Line number to insert/replace at (1-based).")] int lineNumber,
            [Description("Content to insert/replace.")] string content,
            [Description("If true, replaces the line; if false, inserts a new line.")] bool replace = false)
        {
            ToolResult<string> result = await _databaseTool.AddLineAsync(filePath, lineNumber, content, replace);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Search for text within the JSON database file and return matching 1-based line numbers.")]
        public async Task<string> SearchInDatabase(
            [Description("Path to the JSON file, relative to the database directory.")] string filePath,
            [Description("Text to search for (case-insensitive).")] string searchText)
        {
            ToolResult<IEnumerable<int>> result = await _databaseTool.SearchInDatabaseAsync(filePath, searchText);
            return SerializeResult(result);
        }
    }
}
