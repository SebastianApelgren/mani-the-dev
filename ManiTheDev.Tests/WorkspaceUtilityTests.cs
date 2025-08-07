using ManiTheDev.Utilities;

namespace ManiTheDev.Tests;

/// <summary>
/// Unit tests for the WorkspaceUtility class.
/// </summary>
public class WorkspaceUtilityTests : IDisposable
{
    private readonly string _testWorkspacePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspaceUtilityTests"/> class.
    /// </summary>
    public WorkspaceUtilityTests()
    {
        _testWorkspacePath = Path.Combine(Path.GetTempPath(), "WorkspaceUtilityTests");
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
    public void SetupWorkspace_NewWorkspace_CreatesAllDirectories()
    {
        // Arrange & Act
        var (workspacePath, codePath, databasePath) = WorkspaceUtility.SetupWorkspace(_testWorkspacePath);

        // Assert
        Assert.True(Directory.Exists(workspacePath), "Workspace directory should exist");
        Assert.True(Directory.Exists(codePath), "Code directory should exist");
        Assert.True(Directory.Exists(databasePath), "Database directory should exist");

        // Verify paths are correct
        Assert.Equal(_testWorkspacePath, workspacePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "code"), codePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "database"), databasePath);
    }

    [Fact]
    public void SetupWorkspace_ExistingWorkspace_DoesNotThrowException()
    {
        // Arrange - Create directories first
        Directory.CreateDirectory(_testWorkspacePath);
        Directory.CreateDirectory(Path.Combine(_testWorkspacePath, "code"));
        Directory.CreateDirectory(Path.Combine(_testWorkspacePath, "database"));

        // Act & Assert - Should not throw
        var (workspacePath, codePath, databasePath) = WorkspaceUtility.SetupWorkspace(_testWorkspacePath);

        // Verify paths are still correct
        Assert.Equal(_testWorkspacePath, workspacePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "code"), codePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "database"), databasePath);
    }

    [Fact]
    public void SetupWorkspace_PartialExistingWorkspace_CreatesMissingDirectories()
    {
        // Arrange - Create only workspace directory
        Directory.CreateDirectory(_testWorkspacePath);

        // Act
        var (workspacePath, codePath, databasePath) = WorkspaceUtility.SetupWorkspace(_testWorkspacePath);

        // Assert
        Assert.True(Directory.Exists(workspacePath), "Workspace directory should exist");
        Assert.True(Directory.Exists(codePath), "Code directory should be created");
        Assert.True(Directory.Exists(databasePath), "Database directory should be created");

        // Verify paths are correct
        Assert.Equal(_testWorkspacePath, workspacePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "code"), codePath);
        Assert.Equal(Path.Combine(_testWorkspacePath, "database"), databasePath);
    }
}
