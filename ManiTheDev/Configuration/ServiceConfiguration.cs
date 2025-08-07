using EasyReasy.EnvironmentVariables;
using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Tools;
using ManiTheDev.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ManiTheDev.Configuration
{
    /// <summary>
    /// Handles the configuration and registration of all services in the DI container.
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Registers all services in the DI container.
        /// </summary>
        /// <param name="services">The service collection to register services in.</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            // Step 1: Validate environment variables at startup
            EnvironmentVariableHelper.ValidateVariableNamesIn(typeof(EnvironmentVariable));

            // Step 2: Get environment variables
            string openAiApiKey = EnvironmentVariable.OpenAiApiKey.GetValue();
            string workspacePath = EnvironmentVariable.WorkspacePath.GetValue();

            // Step 3: Setup workspace directory structure
            var (workspacePathFull, codePath, databasePath) = WorkspaceUtility.SetupWorkspace(workspacePath);

            // Step 4: Register logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Step 5: Register tools
            RegisterTools(services, workspacePathFull, codePath, databasePath);

            // Step 6: Register agents
            RegisterAgents(services);

            // Step 7: Register Semantic Kernel and planning
            RegisterSemanticKernel(services, openAiApiKey);
        }

        private static void RegisterTools(IServiceCollection services, string workspacePath, string codePath, string databasePath)
        {
            // Register FileTool - uses the main workspace path for general file operations
            services.AddSingleton<IFileTool>(provider =>
            {
                return new FileTool(workspacePath);
            });

            // Register DatabaseTool - uses the database subdirectory for JSON database operations
            services.AddSingleton<IDatabaseTool>(provider =>
            {
                return new DatabaseTool(databasePath);
            });

            // Register CodeTool - uses the code subdirectory for code generation operations
            services.AddSingleton<ICodeTool>(provider =>
            {
                return new CodeTool(codePath);
            });
        }

        private static void RegisterAgents(IServiceCollection services)
        {
            // // Register DatabaseAgent
            // services.AddSingleton<IDatabaseAgent>(provider =>
            // {
            //    var logger = provider.GetRequiredService<ILogger<DatabaseAgent>>();
            //    var databaseTool = provider.GetRequiredService<IDatabaseTool>();
            //    var fileTool = provider.GetRequiredService<IFileTool>();
            //    return new DatabaseAgent(logger, databaseTool, fileTool);
            // });

            // // Register CodingAgent
            // services.AddSingleton<ICodingAgent>(provider =>
            // {
            //    var logger = provider.GetRequiredService<ILogger<CodingAgent>>();
            //    var codeTool = provider.GetRequiredService<ICodeTool>();
            //    var fileTool = provider.GetRequiredService<IFileTool>();
            //    return new CodingAgent(logger, codeTool, fileTool);
            // });

            // // Register OrchestratorAgent
            // services.AddSingleton<IOrchestratorAgent>(provider =>
            // {
            //    var logger = provider.GetRequiredService<ILogger<OrchestratorAgent>>();
            //    var databaseAgent = provider.GetRequiredService<IDatabaseAgent>();
            //    var codingAgent = provider.GetRequiredService<ICodingAgent>();
            //    var kernel = provider.GetRequiredService<Kernel>();
            //    return new OrchestratorAgent(logger, databaseAgent, codingAgent, kernel);
            // });
        }

        private static void RegisterSemanticKernel(IServiceCollection services, string openAiApiKey)
        {
            // Register Kernel with automatic function calling enabled
            services.AddSingleton<Kernel>(provider =>
            {
                Kernel kernel = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion("gpt-4", openAiApiKey)
                    .Build();

                // Get the tools from DI
                var databaseTool = provider.GetRequiredService<IDatabaseTool>();
                var codeTool = provider.GetRequiredService<ICodeTool>();
                var fileTool = provider.GetRequiredService<IFileTool>();

                // Register all function wrappers with the kernel
                kernel.ImportFunctionsFromObject(new DatabaseToolFunctions(databaseTool));
                kernel.ImportFunctionsFromObject(new CodeToolFunctions(codeTool));
                kernel.ImportFunctionsFromObject(new FileToolFunctions(fileTool));

                return kernel;
            });

            // Register OpenAIPromptExecutionSettings for automatic function calling
            services.AddSingleton<OpenAIPromptExecutionSettings>(provider =>
            {
                return new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                };
            });
        }
    }
}