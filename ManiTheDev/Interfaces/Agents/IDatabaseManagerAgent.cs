using ManiTheDev.Models;

namespace ManiTheDev.Interfaces.Agents;

/// <summary>
/// Database manager agent that handles all database operations
/// </summary>
public interface IDatabaseManagerAgent : IAgent
{
    /// <summary>
    /// Handles database schema operations (create, read, update schema)
    /// </summary>
    /// <param name="operation">The database operation to perform</param>
    /// <returns>Result of the database operation</returns>
    Task<AgentResponse> ExecuteDatabaseOperationAsync(DatabaseOperationRequest request);
} 