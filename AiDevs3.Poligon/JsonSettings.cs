using System.Text.Json;

namespace AiDevs3.Poligon;

public static class JsonSettings
{
    public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = new LowerCaseNamingPolicy(),
        WriteIndented = true
    };
}


public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
        {
            return name;
        }
        return char.ToLower(name[0]) + name.Substring(1);
    }
}