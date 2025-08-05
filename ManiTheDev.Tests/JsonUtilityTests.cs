using ManiTheDev.Utilities;

namespace ManiTheDev.Tests;

public class JsonUtilityTests
{
    [Fact]
    public void ValidateJson_ValidJson_ReturnsTrue()
    {
        // Arrange
        var validJson = "{\"name\":\"test\",\"value\":123}";

        // Act
        var result = JsonUtility.ValidateJson(validJson);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateJson_InvalidJson_ReturnsFalse()
    {
        // Arrange
        var invalidJson = "{invalid json}";

        // Act
        var result = JsonUtility.ValidateJson(invalidJson);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateJson_EmptyContent_ReturnsFalse()
    {
        // Arrange
        var emptyJson = "";

        // Act
        var result = JsonUtility.ValidateJson(emptyJson);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateJson_WhitespaceContent_ReturnsFalse()
    {
        // Arrange
        var whitespaceJson = "   \n\t   ";

        // Act
        var result = JsonUtility.ValidateJson(whitespaceJson);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateJson_ComplexValidJson_ReturnsTrue()
    {
        // Arrange
        var complexJson = @"{
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
        var result = JsonUtility.ValidateJson(complexJson);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FormatJson_ValidJson_ReturnsFormattedJson()
    {
        // Arrange
        var compactJson = "{\"name\":\"test\",\"value\":123}";

        // Act
        var result = JsonUtility.FormatJson(compactJson);

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
        var formattedJson = @"{
  ""name"": ""test"",
  ""value"": 123
}";

        // Act
        var result = JsonUtility.FormatJson(formattedJson);

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
        var invalidJson = "{invalid json}";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => JsonUtility.FormatJson(invalidJson));
        
        Assert.Contains("Invalid JSON content provided", exception.Message);
    }

    [Fact]
    public void FormatJson_EmptyContent_ThrowsArgumentException()
    {
        // Arrange
        var emptyJson = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => JsonUtility.FormatJson(emptyJson));
        
        Assert.Contains("Invalid JSON content provided", exception.Message);
    }

    [Fact]
    public void FormatJson_ComplexJson_ReturnsProperlyFormattedJson()
    {
        // Arrange
        var complexJson = "{\"users\":[{\"id\":1,\"name\":\"John\",\"email\":\"john@example.com\",\"active\":true},{\"id\":2,\"name\":\"Jane\",\"email\":\"jane@example.com\",\"active\":false}],\"metadata\":{\"total\":2,\"version\":\"1.0\"}}";

        // Act
        var result = JsonUtility.FormatJson(complexJson);

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