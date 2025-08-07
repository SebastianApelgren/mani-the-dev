using System.ComponentModel;
using System.Text.Json;
using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Models;
using Microsoft.SemanticKernel;

namespace ManiTheDev.Plugins
{
    /// <summary>
    /// Semantic Kernel native functions that wrap CodeTool operations.
    /// </summary>
    public class CodeToolFunctions
    {
        private readonly ICodeTool _codeTool;

        public CodeToolFunctions(ICodeTool codeTool)
        {
            _codeTool = codeTool;
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

        [KernelFunction, Description("Generate code based on a database schema or structure.")]
        public async Task<string> GenerateFromDatabase(
            [Description("Path to the database file, relative to the database directory.")] string databasePath,
            [Description("Name of the entity to generate code for.")] string entityName)
        {
            ToolResult<string> result = await _codeTool.GenerateFromDatabaseAsync(databasePath, entityName);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Create a hook function for a specific purpose.")]
        public async Task<string> CreateHookFunction(
            [Description("Type of hook to create (e.g., beforeSave, afterDelete).")] string hookType,
            [Description("Comma-separated parameters for the hook function.")] string parametersCsv)
        {
            IEnumerable<string> parameters = parametersCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            ToolResult<string> result = await _codeTool.CreateHookFunctionAsync(hookType, parameters);
            return SerializeResult(result);
        }

        [KernelFunction, Description("Validate generated code syntax for a given language.")]
        public async Task<string> ValidateCode(
            [Description("The code content to validate.")] string code,
            [Description("The programming language (e.g., 'csharp', 'javascript').")] string language)
        {
            ToolResult<string> result = await _codeTool.ValidateCodeAsync(code, language);
            return SerializeResult(result);
        }
    }
}
