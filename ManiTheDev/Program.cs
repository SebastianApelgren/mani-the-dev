using ManiTheDev.Configuration;
using ManiTheDev.Interfaces.Agents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ManiTheDev;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Step 1: Create the DI container
        var services = new ServiceCollection();
        
        // Step 2: Configure services (register all dependencies)
        var workspacePath = Path.Combine(Environment.CurrentDirectory, "workspace");
        var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        
        if (string.IsNullOrEmpty(openAiApiKey))
        {
            Console.WriteLine("Error: OPENAI_API_KEY environment variable is not set.");
            Console.WriteLine("Please set your OpenAI API key as an environment variable.");
            return;
        }
        
        ServiceConfiguration.ConfigureServices(services, workspacePath, openAiApiKey);
        
        // Step 3: Build the service provider (DI container)
        using var serviceProvider = services.BuildServiceProvider();
        
        // Step 4: Get the orchestrator agent from the DI container
        var orchestratorAgent = serviceProvider.GetRequiredService<IOrchestratorAgent>();
        
        // Step 5: Use the agent (example)
        Console.WriteLine("AI Agent System Ready!");
        Console.WriteLine($"Workspace: {workspacePath}");
        Console.WriteLine("Enter your goal (or 'exit' to quit):");
        
        while (true)
        {
            Console.Write("> ");
            var userGoal = Console.ReadLine();
            
            if (string.IsNullOrEmpty(userGoal) || userGoal.ToLower() == "exit")
                break;
            
            try
            {
                var result = await orchestratorAgent.ProcessUserGoalAsync(userGoal);
                Console.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
