namespace ManiTheDev.Interfaces.Agents
{
    /// <summary>
    /// Base interface for all agents in the system.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Executes the agent's work based on the provided request.
        /// </summary>
        /// <param name="request">The work request to process.</param>
        /// <returns>Result of the agent's work.</returns>
        Task<string> ExecuteAsync(string request);
    }
}