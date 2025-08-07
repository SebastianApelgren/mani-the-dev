using System;
using System.IO;

namespace ManiTheDev.Utilities
{
    /// <summary>
    /// Utility class for workspace directory setup and management.
    /// </summary>
    public static class WorkspaceUtility
    {
        /// <summary>
        /// Creates the workspace directory structure with subdirectories for code and database files.
        /// </summary>
        /// <param name="workspacePath">The base workspace path.</param>
        /// <returns>A tuple containing the workspace path, code directory path, and database directory path.</returns>
        public static (string WorkspacePath, string CodePath, string DatabasePath) SetupWorkspace(string workspacePath)
        {
            // Ensure the main workspace directory exists
            if (!Directory.Exists(workspacePath))
            {
                Directory.CreateDirectory(workspacePath);
            }

            // Create subdirectories
            string codePath = Path.Combine(workspacePath, "code");
            string databasePath = Path.Combine(workspacePath, "database");

            if (!Directory.Exists(codePath))
            {
                Directory.CreateDirectory(codePath);
            }

            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
            }

            return (workspacePath, codePath, databasePath);
        }
    }
}
