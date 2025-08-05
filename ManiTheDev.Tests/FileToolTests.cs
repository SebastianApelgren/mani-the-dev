using ManiTheDev.Tools;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManiTheDev.Tests;

public class FileToolTests : IDisposable
{
    private readonly Mock<ILogger<FileTool>> _mockLogger;
    private readonly string _testBaseDirectory;
    private readonly FileTool _fileTool;

    public FileToolTests()
    {
        _mockLogger = new Mock<ILogger<FileTool>>();
        _testBaseDirectory = Path.Combine(Path.GetTempPath(), "FileToolTests");
        Directory.CreateDirectory(_testBaseDirectory);
        _fileTool = new FileTool(_mockLogger.Object, _testBaseDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testBaseDirectory))
        {
            Directory.Delete(_testBaseDirectory, true);
        }
    }

    [Fact]
    public async Task ReadFileAsync_ExistingFile_ReturnsContent()
    {
        // Arrange
        var testFilePath = "test.txt";
        var expectedContent = "Hello, World!";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, expectedContent);

        // Act
        var result = await _fileTool.ReadFileAsync(testFilePath);

        // Assert
        Assert.Equal(expectedContent, result);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Reading file")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task ReadFileAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var testFilePath = "nonexistent.txt";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => _fileTool.ReadFileAsync(testFilePath));
        
        Assert.Contains("File not found", exception.Message);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("File does not exist")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task WriteFileAsync_NewFile_CreatesFileWithContent()
    {
        // Arrange
        var testFilePath = "write_test.txt";
        var content = "Test content for writing";

        // Act
        await _fileTool.WriteFileAsync(testFilePath, content);

        // Assert
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        var writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(content, writtenContent);
    }

    [Fact]
    public async Task WriteFileAsync_ExistingFile_OverwritesContent()
    {
        // Arrange
        var testFilePath = "overwrite_test.txt";
        var originalContent = "Original content";
        var newContent = "New content";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, originalContent);

        // Act
        await _fileTool.WriteFileAsync(testFilePath, newContent);

        // Assert
        var writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(newContent, writtenContent);
    }

    [Fact]
    public async Task WriteFileAsync_NonExistentDirectory_CreatesDirectory()
    {
        // Arrange
        var testFilePath = "subdir/test.txt";
        var content = "Test content";

        // Act
        await _fileTool.WriteFileAsync(testFilePath, content);

        // Assert
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        var writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(content, writtenContent);
    }

    [Fact]
    public async Task CreateFileAsync_NewFile_CreatesFileWithContent()
    {
        // Arrange
        var testFilePath = "create_test.txt";
        var content = "Test content for creation";

        // Act
        await _fileTool.CreateFileAsync(testFilePath, content);

        // Assert
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        var writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(content, writtenContent);
    }

    [Fact]
    public async Task CreateFileAsync_ExistingFile_ThrowsInvalidOperationException()
    {
        // Arrange
        var testFilePath = "existing_file.txt";
        var content = "Test content";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, content);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _fileTool.CreateFileAsync(testFilePath, content));
        
        Assert.Contains("File already exists", exception.Message);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("File already exists")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task DeleteFileAsync_ExistingFile_DeletesFile()
    {
        // Arrange
        var testFilePath = "delete_test.txt";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, "Test content");

        // Act
        await _fileTool.DeleteFileAsync(testFilePath);

        // Assert
        Assert.False(File.Exists(fullPath));
    }

    [Fact]
    public async Task DeleteFileAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var testFilePath = "nonexistent_delete.txt";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => _fileTool.DeleteFileAsync(testFilePath));
        
        Assert.Contains("File not found", exception.Message);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("File does not exist")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task ListFilesAsync_ExistingDirectory_ReturnsFileList()
    {
        // Arrange
        var testDir = "list_test_dir";
        var fullDirPath = Path.Combine(_testBaseDirectory, testDir);
        Directory.CreateDirectory(fullDirPath);
        
        var files = new[] { "file1.txt", "file2.txt", "file3.txt" };
        foreach (var file in files)
        {
            await File.WriteAllTextAsync(Path.Combine(fullDirPath, file), "content");
        }

        // Act
        var result = await _fileTool.ListFilesAsync(testDir);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(files.Length, resultList.Count);
        foreach (var file in files)
        {
            Assert.Contains(file, resultList);
        }
    }

    [Fact]
    public async Task ListFilesAsync_NonExistentDirectory_ReturnsEmptyEnumerable()
    {
        // Arrange
        var testDir = "nonexistent_dir";

        // Act
        var result = await _fileTool.ListFilesAsync(testDir);

        // Assert
        Assert.Empty(result);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Directory does not exist")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task AddLineAsync_ValidLineNumber_InsertsLine()
    {
        // Arrange
        var testFilePath = "add_line_test.txt";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var originalLines = new[] { "Line 1", "Line 2", "Line 3" };
        await File.WriteAllLinesAsync(fullPath, originalLines);

        // Act
        await _fileTool.AddLineAsync(testFilePath, 2, "New Line");

        // Assert
        var resultLines = await File.ReadAllLinesAsync(fullPath);
        Assert.Equal(4, resultLines.Length);
        Assert.Equal("Line 1", resultLines[0]);
        Assert.Equal("New Line", resultLines[1]);
        Assert.Equal("Line 2", resultLines[2]);
        Assert.Equal("Line 3", resultLines[3]);
    }

    [Fact]
    public async Task AddLineAsync_InvalidLineNumber_ThrowsArgumentException()
    {
        // Arrange
        var testFilePath = "invalid_line_test.txt";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, "Test content");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _fileTool.AddLineAsync(testFilePath, 0, "New Line"));
        
        Assert.Contains("Line number must be 1 or greater", exception.Message);
    }

    [Fact]
    public async Task AddLineAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var testFilePath = "nonexistent_add_line.txt";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => _fileTool.AddLineAsync(testFilePath, 1, "New Line"));
        
        Assert.Contains("File not found", exception.Message);
    }

    [Fact]
    public async Task SearchInFileAsync_ExistingText_ReturnsMatchingLineNumbers()
    {
        // Arrange
        var testFilePath = "search_test.txt";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var lines = new[] { "First line", "Second line with search", "Third line", "Fourth line with search" };
        await File.WriteAllLinesAsync(fullPath, lines);

        // Act
        var result = await _fileTool.SearchInFileAsync(testFilePath, "search");

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Contains(2, resultList); // "Second line with search"
        Assert.Contains(4, resultList); // "Fourth line with search"
    }

    [Fact]
    public async Task SearchInFileAsync_NonExistentText_ReturnsEmptyEnumerable()
    {
        // Arrange
        var testFilePath = "search_empty_test.txt";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var lines = new[] { "First line", "Second line", "Third line" };
        await File.WriteAllLinesAsync(fullPath, lines);

        // Act
        var result = await _fileTool.SearchInFileAsync(testFilePath, "nonexistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchInFileAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var testFilePath = "nonexistent_search.txt";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => _fileTool.SearchInFileAsync(testFilePath, "search"));
        
        Assert.Contains("File not found", exception.Message);
    }

    [Fact]
    public async Task SearchInFileAsync_CaseInsensitiveSearch_ReturnsMatches()
    {
        // Arrange
        var testFilePath = "case_search_test.txt";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var lines = new[] { "First line", "SECOND LINE", "Third line", "fourth line" };
        await File.WriteAllLinesAsync(fullPath, lines);

        // Act
        var result = await _fileTool.SearchInFileAsync(testFilePath, "line");

        // Assert
        var resultList = result.ToList();
        Assert.Equal(4, resultList.Count);
        Assert.Contains(1, resultList);
        Assert.Contains(2, resultList);
        Assert.Contains(3, resultList);
        Assert.Contains(4, resultList);
    }

    [Fact]
    public async Task Constructor_WithBaseDirectory_SetsBaseDirectory()
    {
        // Arrange & Act
        var customBaseDir = Path.Combine(Path.GetTempPath(), "CustomBaseDir");
        var fileTool = new FileTool(_mockLogger.Object, customBaseDir);

        // Assert
        // We can't directly access the private field, but we can test it indirectly
        // by creating a file and checking it's in the right location
        var testFilePath = "constructor_test.txt";
        var content = "Test content";
        await fileTool.WriteFileAsync(testFilePath, content);

        var expectedPath = Path.Combine(customBaseDir, testFilePath);
        Assert.True(File.Exists(expectedPath));

        // Cleanup
        if (Directory.Exists(customBaseDir))
        {
            Directory.Delete(customBaseDir, true);
        }
    }
} 