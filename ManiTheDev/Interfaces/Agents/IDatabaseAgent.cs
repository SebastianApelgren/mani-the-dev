using ManiTheDev.Models;

namespace ManiTheDev.Interfaces.Agents
{
    /// <summary>
    /// Database agent that handles all database operations.
    /// </summary>
    internal interface IDatabaseAgent
    {
        /// <summary>
        /// Handles database schema operations (create, read, update schema).
        /// </summary>
        /// <param name="request">The database operation request containing the operation details.</param>
        /// <returns>Result of the database operation.</returns>
        Task<AgentResponse> ExecuteDatabaseOperationAsync(DatabaseOperationRequest request);
    }
}