using ManiTheDev.Models;

namespace ManiTheDev.Interfaces.Agents;

/// <summary>
/// Coding agent that handles code generation and validation
/// </summary>
public interface ICodingAgent : IAgent
{
    /// <summary>
    /// Handles code generation operations (generate from database, create hooks, validate code)
    /// </summary>
    /// <param name="operation">The code generation operation to perform</param>
    /// <returns>Result of the code generation operation</returns>
    Task<AgentResponse> ExecuteCodeGenerationAsync(CodeGenerationRequest request);
} 