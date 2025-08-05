using System.Text.Json;

namespace ManiTheDev.Utilities;

public static class JsonUtility
{
    /// <summary>
    /// Validates if the JSON content is valid
    /// </summary>
    /// <param name="jsonContent">JSON content to validate</param>
    /// <returns>True if valid JSON, false otherwise</returns>
    public static bool ValidateJson(string jsonContent)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return false;
            }

            using var document = JsonDocument.Parse(jsonContent);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Formats JSON content for better readability
    /// </summary>
    /// <param name="jsonContent">JSON content to format</param>
    /// <returns>Formatted JSON content</returns>
    public static string FormatJson(string jsonContent)
    {
        if (!ValidateJson(jsonContent))
        {
            throw new ArgumentException("Invalid JSON content provided");
        }

        using var document = JsonDocument.Parse(jsonContent);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        return JsonSerializer.Serialize(document, options);
    }
} 