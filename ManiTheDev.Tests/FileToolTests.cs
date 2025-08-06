using ManiTheDev.Models;
using ManiTheDev.Tools;

namespace ManiTheDev.Tests;

/// <summary>
/// Unit tests for the FileTool class.
/// </summary>
public class FileToolTests : IDisposable
{
    private readonly string _testBaseDirectory;
    private readonly FileTool _fileTool;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileToolTests"/> class.
    /// </summary>
    public FileToolTests()
    {
        _testBaseDirectory = Path.Combine(Path.GetTempPath(), "FileToolTests");
        Directory.CreateDirectory(_testBaseDirectory);
        _fileTool = new FileTool(_testBaseDirectory);
    }

    /// <summary>
    /// Disposes of the test resources.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_testBaseDirectory))
        {
            Directory.Delete(_testBaseDirectory, true);
        }
    }

    [Fact]
    public async Task ReadFileAsync_ExistingFile_ReturnsSuccessWithContent()
    {
        // Arrange
        string testFilePath = "test.txt";
        string expectedContent = "Hello, World!";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, expectedContent);

        // Act
        ToolResult<string> result = await _fileTool.ReadFileAsync(testFilePath);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(expectedContent, result.Data);
        Assert.Contains("Successfully read file", result.Message);
    }

    [Fact]
    public async Task ReadFileAsync_NonExistentFile_ReturnsFailure()
    {
        // Arrange
        string testFilePath = "nonexistent.txt";

        // Act
        ToolResult<string> result = await _fileTool.ReadFileAsync(testFilePath);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("File not found", result.Error);
        Assert.Contains("Failed to read file", result.Message);
    }

    [Fact]
    public async Task WriteFileAsync_NewFile_ReturnsSuccess()
    {
        // Arrange
        string testFilePath = "write_test.txt";
        string content = "Test content for writing";

        // Act
        ToolResult<string> result = await _fileTool.WriteFileAsync(testFilePath, content);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(content, result.Data);
        Assert.Contains("Successfully wrote file", result.Message);

        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        string writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(content, writtenContent);
    }

    [Fact]
    public async Task WriteFileAsync_ExistingFile_OverwritesContent()
    {
        // Arrange
        string testFilePath = "overwrite_test.txt";
        string initialContent = "Initial content";
        string newContent = "New content";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, initialContent);

        // Act
        ToolResult<string> result = await _fileTool.WriteFileAsync(testFilePath, newContent);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(newContent, result.Data);
        Assert.Contains("Successfully wrote file", result.Message);

        string writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(newContent, writtenContent);
    }

    [Fact]
    public async Task WriteFileAsync_NonExistentDirectory_CreatesDirectory()
    {
        // Arrange
        string testFilePath = "subdirectory/write_test.txt";
        string content = "Test content";

        // Act
        ToolResult<string> result = await _fileTool.WriteFileAsync(testFilePath, content);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(content, result.Data);

        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        string writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(content, writtenContent);
    }

    [Fact]
    public async Task CreateFileAsync_NewFile_ReturnsSuccess()
    {
        // Arrange
        string testFilePath = "create_test.txt";
        string content = "Test content for creation";

        // Act
        ToolResult<string> result = await _fileTool.CreateFileAsync(testFilePath, content);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(content, result.Data);
        Assert.Contains("Successfully created file", result.Message);

        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        string writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(content, writtenContent);
    }

    [Fact]
    public async Task CreateFileAsync_ExistingFile_ReturnsFailure()
    {
        // Arrange
        string testFilePath = "existing_create_test.txt";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        string initialContent = "Initial content";
        await File.WriteAllTextAsync(fullPath, initialContent);

        // Act
        ToolResult<string> result = await _fileTool.CreateFileAsync(testFilePath, "New content");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("File already exists", result.Error);
        Assert.Contains("Failed to create file", result.Message);

        // Verify the original content wasn't changed
        string existingContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(initialContent, existingContent);
    }

    [Fact]
    public async Task DeleteFileAsync_ExistingFile_ReturnsSuccess()
    {
        // Arrange
        string testFilePath = "delete_test.txt";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        string content = "Content to delete";
        await File.WriteAllTextAsync(fullPath, content);

        // Act
        ToolResult<string> result = await _fileTool.DeleteFileAsync(testFilePath);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Data);
        Assert.Contains("Successfully deleted file", result.Message);
        Assert.False(File.Exists(fullPath));
    }

    [Fact]
    public async Task DeleteFileAsync_NonExistentFile_ReturnsFailure()
    {
        // Arrange
        string testFilePath = "nonexistent_delete.txt";

        // Act
        ToolResult<string> result = await _fileTool.DeleteFileAsync(testFilePath);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("File not found", result.Error);
        Assert.Contains("Failed to delete file", result.Message);
    }

    [Fact]
    public async Task ListFilesAsync_ExistingDirectory_ReturnsSuccessWithFileList()
    {
        // Arrange
        string testDirectory = "list_test_dir";
        string fullDirPath = Path.Combine(_testBaseDirectory, testDirectory);
        Directory.CreateDirectory(fullDirPath);

        string[] files = new[] { "file1.txt", "file2.txt", "file3.txt" };
        foreach (string? file in files)
        {
            await File.WriteAllTextAsync(Path.Combine(fullDirPath, file), "content");
        }

        // Act
        ToolResult<IEnumerable<string>> result = await _fileTool.ListFilesAsync(testDirectory);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        List<string> fileList = result.Data!.ToList();
        Assert.Equal(3, fileList.Count);
        Assert.Contains("file1.txt", fileList);
        Assert.Contains("file2.txt", fileList);
        Assert.Contains("file3.txt", fileList);
        Assert.Contains("Successfully listed files", result.Message);
    }

    [Fact]
    public async Task ListFilesAsync_NonExistentDirectory_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        string testDirectory = "nonexistent_list_dir";

        // Act
        ToolResult<IEnumerable<string>> result = await _fileTool.ListFilesAsync(testDirectory);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data!);
        Assert.Contains("Directory not found", result.Message);
    }

    [Fact]
    public async Task AddLineAsync_ValidLineNumber_ReturnsSuccess()
    {
        // Arrange
        string testFilePath = "add_line_test.txt";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        string initialContent = "Line 1\nLine 2\nLine 3";
        await File.WriteAllTextAsync(fullPath, initialContent);

        // Act
        ToolResult<string> result = await _fileTool.AddLineAsync(testFilePath, 2, "New line");

        // Assert
        Assert.True(result.Success);
        Assert.Contains("Successfully added line 2", result.Message);

        string[] lines = await File.ReadAllLinesAsync(fullPath);
        Assert.Equal(4, lines.Length);
        Assert.Equal("Line 1", lines[0]);
        Assert.Equal("New line", lines[1]);
        Assert.Equal("Line 2", lines[2]);
        Assert.Equal("Line 3", lines[3]);
    }

    [Fact]
    public async Task AddLineAsync_InvalidLineNumber_ReturnsFailure()
    {
        // Arrange
        string testFilePath = "invalid_line_test.txt";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, "Line 1\nLine 2");

        // Act
        ToolResult<string> result = await _fileTool.AddLineAsync(testFilePath, 0, "New line");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Line number must be 1 or greater", result.Error);
        Assert.Contains("Failed to modify file", result.Message);
    }

    [Fact]
    public async Task AddLineAsync_NonExistentFile_ReturnsFailure()
    {
        // Arrange
        string testFilePath = "nonexistent_add_line.txt";

        // Act
        ToolResult<string> result = await _fileTool.AddLineAsync(testFilePath, 1, "New line");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("File not found", result.Error);
        Assert.Contains("Failed to modify file", result.Message);
    }

    [Fact]
    public async Task SearchInFileAsync_ExistingText_ReturnsSuccessWithMatchingLineNumbers()
    {
        // Arrange
        string testFilePath = "search_test.txt";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        string content = "First line\nSecond line with search\nThird line\nFourth line with search";
        await File.WriteAllTextAsync(fullPath, content);

        // Act
        ToolResult<IEnumerable<int>> result = await _fileTool.SearchInFileAsync(testFilePath, "search");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        List<int> lineNumbers = result.Data!.ToList();
        Assert.Equal(2, lineNumbers.Count);
        Assert.Contains(2, lineNumbers);
        Assert.Contains(4, lineNumbers);
        Assert.Contains("Found 2 matching lines", result.Message);
    }

    [Fact]
    public async Task SearchInFileAsync_NonExistentText_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        string testFilePath = "search_empty_test.txt";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        string content = "First line\nSecond line\nThird line";
        await File.WriteAllTextAsync(fullPath, content);

        // Act
        ToolResult<IEnumerable<int>> result = await _fileTool.SearchInFileAsync(testFilePath, "nonexistent");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data!);
        Assert.Contains("Found 0 matching lines", result.Message);
    }

    [Fact]
    public async Task SearchInFileAsync_NonExistentFile_ReturnsFailure()
    {
        // Arrange
        string testFilePath = "nonexistent_search.txt";

        // Act
        ToolResult<IEnumerable<int>> result = await _fileTool.SearchInFileAsync(testFilePath, "search");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("File not found", result.Error);
        Assert.Contains("Failed to search file", result.Message);
    }

    [Fact]
    public async Task SearchInFileAsync_CaseInsensitiveSearch_ReturnsSuccessWithMatches()
    {
        // Arrange
        string testFilePath = "case_insensitive_test.txt";
        string fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        string content = "First line\nSecond line with SEARCH\nThird line\nFourth line with search";
        await File.WriteAllTextAsync(fullPath, content);

        // Act
        ToolResult<IEnumerable<int>> result = await _fileTool.SearchInFileAsync(testFilePath, "search");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        List<int> lineNumbers = result.Data!.ToList();
        Assert.Equal(2, lineNumbers.Count);
        Assert.Contains(2, lineNumbers);
        Assert.Contains(4, lineNumbers);
        Assert.Contains("Found 2 matching lines", result.Message);
    }

    [Fact]
    public async Task Constructor_WithBaseDirectory_SetsBaseDirectory()
    {
        // Arrange & Act
        string customBaseDir = Path.Combine(Path.GetTempPath(), "CustomFileBaseDir");
        FileTool fileTool = new FileTool(customBaseDir);

        // Assert
        // Test it indirectly by creating a file
        string testFilePath = "constructor_test.txt";
        string content = "Test content";
        ToolResult<string> result = await fileTool.WriteFileAsync(testFilePath, content);

        Assert.True(result.Success);
        string expectedPath = Path.Combine(customBaseDir, testFilePath);
        Assert.True(File.Exists(expectedPath));

        // Cleanup
        if (Directory.Exists(customBaseDir))
        {
            Directory.Delete(customBaseDir, true);
        }
    }
}