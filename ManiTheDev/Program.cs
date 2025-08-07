using EasyReasy.EnvironmentVariables;
using ManiTheDev.Configuration;
using ManiTheDev.Interfaces.Agents;
using Microsoft.Extensions.DependencyInjection;

namespace ManiTheDev
{
    /// <summary>
    /// Main program class that orchestrates the AI agent system.
    /// </summary>
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                // Step 1: Load environment variables from .env file if it exists
                string envFilePath = Path.Combine(Environment.CurrentDirectory, ".env");
                if (File.Exists(envFilePath))
                {
                    EnvironmentVariableHelper.LoadVariablesFromFile(envFilePath);
                    Console.WriteLine("Loaded environment variables from .env file");
                }

                // Step 2: Create the DI container
                ServiceCollection services = new ServiceCollection();

                // Step 3: Configure services (register all dependencies)
                // This will validate environment variables and create workspace directory
                ServiceConfiguration.ConfigureServices(services);

                // Step 4: Build the service provider (DI container)
                using ServiceProvider serviceProvider = services.BuildServiceProvider();

                // Step 5: Get the orchestrator agent from the DI container
                IOrchestratorAgent orchestratorAgent = serviceProvider.GetRequiredService<IOrchestratorAgent>();

                // Step 6: Use the agent (example)
                Console.WriteLine("AI Agent System Ready!");
                Console.WriteLine($"Workspace: {EnvironmentVariable.WorkspacePath.GetValue()}");
                Console.WriteLine("Enter your goal (or 'exit' to quit):");

                while (true)
                {
                    Console.Write("> ");
                    string? userGoal = Console.ReadLine();

                    if (string.IsNullOrEmpty(userGoal) || userGoal.ToLower() == "exit")
                    {
                        break;
                    }

                    try
                    {
                        string result = await orchestratorAgent.ProcessUserGoalAsync(userGoal);
                        Console.WriteLine($"Result: {result}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Configuration Error: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Please ensure all required environment variables are set:");
                Console.WriteLine("- OPENAI_API_KEY: Your OpenAI API key");
                Console.WriteLine("- AGENT_WORKSPACE_PATH: Path for agent workspace");
                Console.WriteLine();
                Console.WriteLine("See Configuration/EnvironmentSetup.md for setup instructions.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }
    }
}