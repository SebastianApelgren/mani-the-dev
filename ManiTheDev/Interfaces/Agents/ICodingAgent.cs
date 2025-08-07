using ManiTheDev.Models;

namespace ManiTheDev.Interfaces.Agents
{
    /// <summary>
    /// Coding agent that handles code generation and validation.
    /// </summary>
    internal interface ICodingAgent
    {
        /// <summary>
        /// Handles code generation operations (generate from database, create hooks, validate code).
        /// </summary>
        /// <param name="request">The code generation request containing the operation details.</param>
        /// <returns>Result of the code generation operation.</returns>
        Task<AgentResponse> ExecuteCodeGenerationAsync(CodeGenerationRequest request);
    }
}