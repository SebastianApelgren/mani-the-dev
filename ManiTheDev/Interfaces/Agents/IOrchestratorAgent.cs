namespace ManiTheDev.Interfaces.Agents
{
    /// <summary>
    /// Orchestrator agent that plans and delegates work to other agents.
    /// </summary>
    public interface IOrchestratorAgent : IAgent
    {
        /// <summary>
        /// Processes a user goal by creating a plan and delegating to appropriate agents.
        /// </summary>
        /// <param name="userGoal">The user's goal or request.</param>
        /// <returns>Summary of the completed work.</returns>
        Task<string> ProcessUserGoalAsync(string userGoal);
    }
}