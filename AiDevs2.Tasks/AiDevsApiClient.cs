using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiDevs2.Tasks;

public class AiDevsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AiDevsApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AiDevsTasks:ApiKey"] ?? throw new InvalidOperationException("Missing configurationAiDevsTasks:ApiKey");
    }

    public async Task<string> GetAuthenticationToken(string taskName)
    {
        var response = await _httpClient.PostAsync($"https://tasks.aidevs.pl/token/{taskName}",
            new StringContent(JsonSerializer.Serialize(new { apikey = _apiKey }), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content);
        return json.GetProperty("token").GetString() ?? throw new InvalidOperationException("No token in response");
    }

    public async Task<string> GetTask(string token)
    {
        var response = await _httpClient.GetAsync($"https://tasks.aidevs.pl/task/{token}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task SubmitAnswer(string token, string answer)
    {
        var response = await _httpClient.PostAsync($"https://tasks.aidevs.pl/answer/{token}",
            new StringContent(JsonSerializer.Serialize(new { answer }), Encoding.UTF8, "application/json"));

        Console.WriteLine(response.IsSuccessStatusCode
            ? "OK"
            : "ERROR");
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddAiDevsApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<AiDevsApiClient>();
    }
}