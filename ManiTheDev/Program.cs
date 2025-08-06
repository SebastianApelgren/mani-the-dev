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
            // Step 1: Create the DI container
            ServiceCollection services = new ServiceCollection();

            // Step 2: Configure services (register all dependencies)
            string workspacePath = Path.Combine(Environment.CurrentDirectory, "workspace");
            string? openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(openAiApiKey))
            {
                Console.WriteLine("Error: OPENAI_API_KEY environment variable is not set.");
                Console.WriteLine("Please set your OpenAI API key as an environment variable.");
                return;
            }

            ServiceConfiguration.ConfigureServices(services, workspacePath, openAiApiKey);

            // Step 3: Build the service provider (DI container)
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            // Step 4: Get the orchestrator agent from the DI container
            IOrchestratorAgent orchestratorAgent = serviceProvider.GetRequiredService<IOrchestratorAgent>();

            // Step 5: Use the agent (example)
            Console.WriteLine("AI Agent System Ready!");
            Console.WriteLine($"Workspace: {workspacePath}");
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
    }
}
