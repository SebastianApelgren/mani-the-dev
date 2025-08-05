using ManiTheDev.Tools;
using ManiTheDev.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManiTheDev.Tests;

public class DatabaseToolTests : IDisposable
{
    private readonly Mock<ILogger<DatabaseTool>> _mockLogger;
    private readonly string _testBaseDirectory;
    private readonly DatabaseTool _databaseTool;

    public DatabaseToolTests()
    {
        _mockLogger = new Mock<ILogger<DatabaseTool>>();
        _testBaseDirectory = Path.Combine(Path.GetTempPath(), "DatabaseToolTests");
        Directory.CreateDirectory(_testBaseDirectory);
        _databaseTool = new DatabaseTool(_mockLogger.Object, _testBaseDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testBaseDirectory))
        {
            Directory.Delete(_testBaseDirectory, true);
        }
    }

    [Fact]
    public async Task ReadDatabaseAsync_ExistingFile_ReturnsContent()
    {
        // Arrange
        var testFilePath = "test.json";
        var expectedContent = "{\"name\":\"test\",\"value\":123}";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, expectedContent);

        // Act
        var result = await _databaseTool.ReadDatabaseAsync(testFilePath);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(expectedContent, result.Data);
        Assert.Contains("Successfully read database", result.Message);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Reading database")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task ReadDatabaseAsync_NonExistentFile_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "nonexistent.json";

        // Act
        var result = await _databaseTool.ReadDatabaseAsync(testFilePath);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Database file not found", result.Error);
        Assert.Contains("Failed to read database", result.Message);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Database file does not exist")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task WriteDatabaseAsync_ValidJson_CreatesFile()
    {
        // Arrange
        var testFilePath = "write_test.json";
        var jsonContent = "{\"name\":\"test\",\"value\":123}";

        // Act
        var result = await _databaseTool.WriteDatabaseAsync(testFilePath, jsonContent);

        // Assert
        Assert.True(result.Success);
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        var writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(jsonContent, writtenContent);
        Assert.Contains("Successfully wrote database", result.Message);
    }

    [Fact]
    public async Task WriteDatabaseAsync_InvalidJson_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "invalid_test.json";
        var invalidJson = "{invalid json}";

        // Act
        var result = await _databaseTool.WriteDatabaseAsync(testFilePath, invalidJson);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid JSON content provided", result.Error);
        Assert.Contains("Failed to write database", result.Message);
    }

    [Fact]
    public async Task CreateDatabaseAsync_NewFile_CreatesFileWithContent()
    {
        // Arrange
        var testFilePath = "create_test.json";
        var jsonContent = "{\"name\":\"test\",\"value\":123}";

        // Act
        var result = await _databaseTool.CreateDatabaseAsync(testFilePath, jsonContent);

        // Assert
        Assert.True(result.Success);
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        Assert.True(File.Exists(fullPath));
        var writtenContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(jsonContent, writtenContent);
        Assert.Contains("Successfully created database", result.Message);
    }

    [Fact]
    public async Task CreateDatabaseAsync_ExistingFile_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "existing_file.json";
        var jsonContent = "{\"name\":\"test\"}";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, jsonContent);

        // Act
        var result = await _databaseTool.CreateDatabaseAsync(testFilePath, jsonContent);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Database file already exists", result.Error);
        Assert.Contains("Failed to create database", result.Message);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Database file already exists")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task AddLineAsync_ValidLineNumber_InsertsLine()
    {
        // Arrange
        var testFilePath = "add_line_test.json";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var originalJson = "{\n  \"name\": \"test\",\n  \"value\": 123\n}";
        await File.WriteAllTextAsync(fullPath, originalJson);

        // Act
        var result = await _databaseTool.AddLineAsync(testFilePath, 2, "  \"newProperty\": \"value\",");

        // Assert
        Assert.True(result.Success);
        var resultLines = await File.ReadAllLinesAsync(fullPath);
        Assert.Equal(5, resultLines.Length);
        Assert.Equal("{", resultLines[0]);
        Assert.Equal("  \"newProperty\": \"value\",", resultLines[1]);
        Assert.Equal("  \"name\": \"test\",", resultLines[2]);
        Assert.Equal("  \"value\": 123", resultLines[3]);
        Assert.Equal("}", resultLines[4]);
        Assert.Contains("Successfully added line", result.Message);
    }

    [Fact]
    public async Task AddLineAsync_ReplaceLine_ReplacesLine()
    {
        // Arrange
        var testFilePath = "replace_line_test.json";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var originalJson = "{\n  \"name\": \"test\",\n  \"value\": 123\n}";
        await File.WriteAllTextAsync(fullPath, originalJson);

        // Act
        var result = await _databaseTool.AddLineAsync(testFilePath, 2, "  \"name\": \"updated\",", replace: true);

        // Assert
        Assert.True(result.Success);
        var resultLines = await File.ReadAllLinesAsync(fullPath);
        Assert.Equal(4, resultLines.Length);
        Assert.Equal("{", resultLines[0]);
        Assert.Equal("  \"name\": \"updated\",", resultLines[1]);
        Assert.Equal("  \"value\": 123", resultLines[2]);
        Assert.Equal("}", resultLines[3]);
        Assert.Contains("Successfully replaced line", result.Message);
    }

    [Fact]
    public async Task AddLineAsync_InvalidLineNumber_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "invalid_line_test.json";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        await File.WriteAllTextAsync(fullPath, "{\n  \"name\": \"test\"\n}");

        // Act
        var result = await _databaseTool.AddLineAsync(testFilePath, 0, "New Line");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Line number must be 1 or greater", result.Error);
        Assert.Contains("Failed to modify database", result.Message);
    }

    [Fact]
    public async Task AddLineAsync_ReplaceNonExistentLine_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "replace_nonexistent_test.json";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var originalJson = "{\n  \"name\": \"test\"\n}";
        await File.WriteAllTextAsync(fullPath, originalJson);

        // Act
        var result = await _databaseTool.AddLineAsync(testFilePath, 5, "New Line", replace: true);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Line 5 does not exist for replacement", result.Error);
        Assert.Contains("Failed to modify database", result.Message);
    }

    [Fact]
    public async Task AddLineAsync_NonExistentFile_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "nonexistent_add_line.json";

        // Act
        var result = await _databaseTool.AddLineAsync(testFilePath, 1, "New Line");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Database file not found", result.Error);
        Assert.Contains("Failed to modify database", result.Message);
    }

    [Fact]
    public async Task AddLineAsync_InvalidJsonAfterModification_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "invalid_json_test.json";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var validJson = "{\n  \"name\": \"test\",\n  \"value\": 123\n}";
        await File.WriteAllTextAsync(fullPath, validJson);

        // Act - Insert content that would break JSON structure
        var result = await _databaseTool.AddLineAsync(testFilePath, 2, "invalid json content");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("The modification would create invalid JSON", result.Error);
        Assert.Contains("Failed to modify database", result.Message);
    }

    [Fact]
    public async Task SearchInDatabaseAsync_ExistingText_ReturnsMatchingLineNumbers()
    {
        // Arrange
        var testFilePath = "search_test.json";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var lines = new[] { "First line", "Second line with search", "Third line", "Fourth line with search" };
        await File.WriteAllLinesAsync(fullPath, lines);

        // Act
        var result = await _databaseTool.SearchInDatabaseAsync(testFilePath, "search");

        // Assert
        Assert.True(result.Success);
        var resultList = result.Data!.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Contains(2, resultList); // "Second line with search"
        Assert.Contains(4, resultList); // "Fourth line with search"
        Assert.Contains("Found 2 matching lines", result.Message);
    }

    [Fact]
    public async Task SearchInDatabaseAsync_NonExistentText_ReturnsEmptyEnumerable()
    {
        // Arrange
        var testFilePath = "search_empty_test.json";
        var fullPath = Path.Combine(_testBaseDirectory, testFilePath);
        var lines = new[] { "First line", "Second line", "Third line" };
        await File.WriteAllLinesAsync(fullPath, lines);

        // Act
        var result = await _databaseTool.SearchInDatabaseAsync(testFilePath, "nonexistent");

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Data!);
        Assert.Contains("Found 0 matching lines", result.Message);
    }

    [Fact]
    public async Task SearchInDatabaseAsync_NonExistentFile_ReturnsFailure()
    {
        // Arrange
        var testFilePath = "nonexistent_search.json";

        // Act
        var result = await _databaseTool.SearchInDatabaseAsync(testFilePath, "search");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Database file not found", result.Error);
        Assert.Contains("Failed to search database", result.Message);
    }

    [Fact]
    public async Task Constructor_WithBaseDirectory_SetsBaseDirectory()
    {
        // Arrange & Act
        var customBaseDir = Path.Combine(Path.GetTempPath(), "CustomDatabaseBaseDir");
        var databaseTool = new DatabaseTool(_mockLogger.Object, customBaseDir);

        // Assert
        // Test it indirectly by creating a database file
        var testFilePath = "constructor_test.json";
        var content = "{\"test\":\"value\"}";
        var result = await databaseTool.WriteDatabaseAsync(testFilePath, content);

        Assert.True(result.Success);
        var expectedPath = Path.Combine(customBaseDir, testFilePath);
        Assert.True(File.Exists(expectedPath));

        // Cleanup
        if (Directory.Exists(customBaseDir))
        {
            Directory.Delete(customBaseDir, true);
        }
    }
} 