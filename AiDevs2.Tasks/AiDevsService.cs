using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AiDevs2.Tasks;

public class AiDevsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://tasks.aidevs.pl";

    public AiDevsService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AiDevsTasks:ApiKey"] 
                  ?? throw new InvalidOperationException("Missing configurationAiDevsTasks:ApiKey");
    }

    public async Task<string> GetAuthenticationToken(string taskName)
    {
        var response = await _httpClient.PostAsync($"{BaseUrl}/token/{taskName}",
            new StringContent(JsonSerializer.Serialize(new { apikey = _apiKey }), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content);
        return json.GetProperty("token").GetString() ?? throw new InvalidOperationException("No token in response");
    }

    public async Task<string> GetTask(string token)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/task/{token}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> SubmitAnswer(string token, string answer)
    {
        var response = await _httpClient.PostAsync($"{BaseUrl}/answer/{token}",
            new StringContent(JsonSerializer.Serialize(new { answer }), Encoding.UTF8, "application/json"));

        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(response.IsSuccessStatusCode ? "OK" : "ERROR");

        return JToken.Parse(content).ToString(Formatting.Indented);
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddAiDevsApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<AiDevsService>();
    }
}

