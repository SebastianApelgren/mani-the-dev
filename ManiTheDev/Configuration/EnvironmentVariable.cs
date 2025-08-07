using EasyReasy.EnvironmentVariables;

namespace ManiTheDev.Configuration
{
    /// <summary>
    /// Defines all environment variables used by the application.
    /// </summary>
    [EnvironmentVariableNameContainer]
    public static class EnvironmentVariable
    {
        /// <summary>
        /// The OpenAI API key required for Semantic Kernel operations.
        /// </summary>
        [EnvironmentVariableName(minLength: 20)]
        public static readonly VariableName OpenAiApiKey = new VariableName("OPENAI_API_KEY");

        /// <summary>
        /// The workspace path where agents will store and manage files.
        /// </summary>
        [EnvironmentVariableName(minLength: 1)]
        public static readonly VariableName WorkspacePath = new VariableName("AGENT_WORKSPACE_PATH");
    }
}