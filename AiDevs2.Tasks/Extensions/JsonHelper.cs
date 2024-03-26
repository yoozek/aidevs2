using System.Text.Json;

namespace AiDevs2.Tasks.Extensions;

public class JsonHelper
{
    public static T? TryDeserialize<T>(string json) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}