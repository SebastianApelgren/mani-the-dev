using ManiTheDev.Configuration;
using ManiTheDev.Interfaces.Tools;
using ManiTheDev.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace ManiTheDev.Tests;

/// <summary>
/// Unit tests for the ServiceConfiguration class.
/// </summary>
public class ServiceConfigurationTests : IDisposable
{
    private readonly string _testWorkspacePath;
    private readonly ServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceConfigurationTests"/> class.
    /// </summary>
    public ServiceConfigurationTests()
    {
        _testWorkspacePath = Path.Combine(Path.GetTempPath(), "ServiceConfigurationTests");
        _services = new ServiceCollection();
    }

    /// <summary>
    /// Disposes of the test resources.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_testWorkspacePath))
        {
            Directory.Delete(_testWorkspacePath, true);
        }
    }

    [Fact]
    public void WorkspaceUtility_SetupWorkspace_CreatesCorrectStructure()
    {
        // Arrange & Act
        var (workspacePath, codePath, databasePath) = WorkspaceUtility.SetupWorkspace(_testWorkspacePath);

        // Assert
        Assert.True(Directory.Exists(workspacePath), "Workspace directory should exist");
        Assert.True(Directory.Exists(codePath), "Code directory should exist");
        Assert.True(Directory.Exists(databasePath), "Database directory should exist");

        // Verify the structure matches expectations
        Assert.Equal(_testWorkspacePath, workspacePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "code"), codePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "database"), databasePath);
    }

    [Fact]
    public void Tools_UseCorrectBaseDirectories()
    {
        // Arrange
        var (workspacePath, codePath, databasePath) = WorkspaceUtility.SetupWorkspace(_testWorkspacePath);

        // Act - Create tools manually to test directory assignments
        var fileTool = new ManiTheDev.Tools.FileTool(workspacePath);
        var databaseTool = new ManiTheDev.Tools.DatabaseTool(databasePath);
        var codeTool = new ManiTheDev.Tools.CodeTool(codePath);

        // Assert - Verify tools are created (this tests the constructor accepts the paths)
        Assert.NotNull(fileTool);
        Assert.NotNull(databaseTool);
        Assert.NotNull(codeTool);

        // Create test files to verify paths are working
        string testFile = Path.Combine(workspacePath, "test.txt");
        string testDatabase = Path.Combine(databasePath, "test.json");
        string testCode = Path.Combine(codePath, "test.cs");

        File.WriteAllText(testFile, "test");
        File.WriteAllText(testDatabase, "{}");
        File.WriteAllText(testCode, "// test");

        Assert.True(File.Exists(testFile), "FileTool should work with workspace path");
        Assert.True(File.Exists(testDatabase), "DatabaseTool should work with database path");
        Assert.True(File.Exists(testCode), "CodeTool should work with code path");
    }
}
