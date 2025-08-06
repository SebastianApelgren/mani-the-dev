using ManiTheDev.Utilities;

namespace ManiTheDev.Tests;

/// <summary>
/// Unit tests for the JsonUtility class.
/// </summary>
public class JsonUtilityTests
{
    [Fact]
    public void ValidateJson_ValidJson_ReturnsTrue()
    {
        // Arrange
        string validJson = "{\"name\":\"test\",\"value\":123}";

        // Act
        bool result = JsonUtility.ValidateJson(validJson);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateJson_InvalidJson_ReturnsFalse()
    {
        // Arrange
        string invalidJson = "{invalid json}";

        // Act
        bool result = JsonUtility.ValidateJson(invalidJson);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateJson_EmptyContent_ReturnsFalse()
    {
        // Arrange
        string emptyJson = "";

        // Act
        bool result = JsonUtility.ValidateJson(emptyJson);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateJson_WhitespaceContent_ReturnsFalse()
    {
        // Arrange
        string whitespaceJson = "   \n\t   ";

        // Act
        bool result = JsonUtility.ValidateJson(whitespaceJson);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateJson_ComplexValidJson_ReturnsTrue()
    {
        // Arrange
        string complexJson = @"{
            ""users"": [
                {
                    ""id"": 1,
                    ""name"": ""John"",
                    ""email"": ""john@example.com"",
                    ""active"": true
                },
                {
                    ""id"": 2,
                    ""name"": ""Jane"",
                    ""email"": ""jane@example.com"",
                    ""active"": false
                }
            ],
            ""metadata"": {
                ""total"": 2,
                ""version"": ""1.0""
            }
        }";

        // Act
        bool result = JsonUtility.ValidateJson(complexJson);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FormatJson_ValidJson_ReturnsFormattedJson()
    {
        // Arrange
        string compactJson = "{\"name\":\"test\",\"value\":123}";

        // Act
        string result = JsonUtility.FormatJson(compactJson);

        // Assert
        Assert.Contains("\n", result); // Should be formatted with newlines
        Assert.Contains("  ", result); // Should be indented
        Assert.Contains("\"name\"", result);
        Assert.Contains("\"value\"", result);
        Assert.Contains("123", result);
    }

    [Fact]
    public void FormatJson_AlreadyFormattedJson_ReturnsFormattedJson()
    {
        // Arrange
        string formattedJson = @"{
  ""name"": ""test"",
  ""value"": 123
}";

        // Act
        string result = JsonUtility.FormatJson(formattedJson);

        // Assert
        Assert.Contains("\n", result);
        Assert.Contains("  ", result);
        Assert.Contains("\"name\"", result);
        Assert.Contains("\"value\"", result);
    }

    [Fact]
    public void FormatJson_InvalidJson_ThrowsArgumentException()
    {
        // Arrange
        string invalidJson = "{invalid json}";

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => JsonUtility.FormatJson(invalidJson));

        Assert.Contains("Invalid JSON content provided", exception.Message);
    }

    [Fact]
    public void FormatJson_EmptyContent_ThrowsArgumentException()
    {
        // Arrange
        string emptyJson = "";

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => JsonUtility.FormatJson(emptyJson));

        Assert.Contains("Invalid JSON content provided", exception.Message);
    }

    [Fact]
    public void FormatJson_ComplexJson_ReturnsProperlyFormattedJson()
    {
        // Arrange
        string complexJson = "{\"users\":[{\"id\":1,\"name\":\"John\",\"email\":\"john@example.com\",\"active\":true},{\"id\":2,\"name\":\"Jane\",\"email\":\"jane@example.com\",\"active\":false}],\"metadata\":{\"total\":2,\"version\":\"1.0\"}}";

        // Act
        string result = JsonUtility.FormatJson(complexJson);

        // Assert
        Assert.Contains("\n", result);
        Assert.Contains("  ", result);
        Assert.Contains("\"users\"", result);
        Assert.Contains("\"metadata\"", result);
        Assert.Contains("\"John\"", result);
        Assert.Contains("\"Jane\"", result);
        Assert.Contains("true", result);
        Assert.Contains("false", result);
    }
}