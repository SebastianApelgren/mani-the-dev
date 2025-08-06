using ManiTheDev.Interfaces;
using ManiTheDev.Interfaces.Agents;
using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ManiTheDev.Configuration;

/// <summary>
/// Handles the configuration and registration of all services in the DI container
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Registers all services in the DI container
    /// </summary>
    /// <param name="services">The service collection to register services in</param>
    /// <param name="workspacePath">The base workspace path for all agents</param>
    /// <param name="openAiApiKey">The OpenAI API key</param>
    public static void ConfigureServices(IServiceCollection services, string workspacePath, string openAiApiKey)
    {
        // Step 1: Register logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Step 2: Register tools
        RegisterTools(services, workspacePath);

        // Step 3: Register agents
        RegisterAgents(services);

        // Step 4: Register Semantic Kernel and planning
        RegisterSemanticKernel(services, openAiApiKey);
    }

    private static void RegisterTools(IServiceCollection services, string workspacePath)
    {
        // Register FileTool - each agent will get the same workspace path
        services.AddSingleton<IFileTool>(provider =>
        {
            return new FileTool(workspacePath);
        });

        // Register DatabaseTool - for JSON database operations
        services.AddSingleton<IDatabaseTool>(provider =>
        {
            return new DatabaseTool(workspacePath);
        });

        // Register CodeTool - placeholder implementation for now
        services.AddSingleton<ICodeTool>(provider =>
        {
            return new CodeTool(workspacePath);
        });
    }

    private static void RegisterAgents(IServiceCollection services)
    {
        //// Register DatabaseManagerAgent
        //services.AddSingleton<IDatabaseManagerAgent>(provider =>
        //{
        //    var logger = provider.GetRequiredService<ILogger<DatabaseManagerAgent>>();
        //    var databaseTool = provider.GetRequiredService<IDatabaseTool>();
        //    var fileTool = provider.GetRequiredService<IFileTool>();
        //    return new DatabaseManagerAgent(logger, databaseTool, fileTool);
        //});

        //// Register CodingAgent
        //services.AddSingleton<ICodingAgent>(provider =>
        //{
        //    var logger = provider.GetRequiredService<ILogger<CodingAgent>>();
        //    var codeTool = provider.GetRequiredService<ICodeTool>();
        //    var fileTool = provider.GetRequiredService<IFileTool>();
        //    return new CodingAgent(logger, codeTool, fileTool);
        //});

        //// Register OrchestratorAgent
        //services.AddSingleton<IOrchestratorAgent>(provider =>
        //{
        //    var logger = provider.GetRequiredService<ILogger<OrchestratorAgent>>();
        //    var databaseAgent = provider.GetRequiredService<IDatabaseManagerAgent>();
        //    var codingAgent = provider.GetRequiredService<ICodingAgent>();
        //    var kernel = provider.GetRequiredService<Kernel>();
        //    return new OrchestratorAgent(logger, databaseAgent, codingAgent, kernel);
        //});
    }

    private static void RegisterSemanticKernel(IServiceCollection services, string openAiApiKey)
    {
        // Register Kernel with automatic function calling enabled
        services.AddSingleton<Kernel>(provider =>
        {
            var kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion("gpt-4", openAiApiKey)
                .Build();
            return kernel;
        });

        // Register OpenAIPromptExecutionSettings for automatic function calling
        services.AddSingleton<OpenAIPromptExecutionSettings>(provider =>
        {
            return new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };
        });
    }
} 