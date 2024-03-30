using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AiDevs2.Tasks.ApiClients;

public record AiDevsClientConfiguration(Uri BaseUrl, string ApiKey);

public class AiDevsClient(HttpClient httpClient, AiDevsClientConfiguration configuration)
{
    public async Task<string> GetAuthenticationToken(string taskName)
    {
        var response = await httpClient.PostAsync($"token/{taskName}",
            new StringContent(JsonSerializer.Serialize(new { apikey = configuration.ApiKey }), Encoding.UTF8,
                "application/json"));
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return responseJson.GetProperty("token").GetString()
               ?? throw new InvalidOperationException("No token in response");
    }

    public async Task<string> GetTask(string token, Dictionary<string, string>? formData = null)
    {
        var response = await httpClient.PostAsync($"task/{token}",
            formData != null ? new FormUrlEncodedContent(formData) : null);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string?> GetHint(string taskName)
    {
        var response = await httpClient.PostAsync($"hint/{taskName}", new StringContent(JsonSerializer.Serialize(new { apikey = configuration.ApiKey }), Encoding.UTF8,
            "application/json"));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> SubmitAnswer(string token, string answer)
    {
        var response = await httpClient.PostAsync($"answer/{token}",
            new StringContent(answer, Encoding.UTF8, "application/json"));

        return await response.Content.ReadAsStringAsync();
    }
}

public static class AiDevsClientServiceCollectionExtensions
{
    public static void AddAiDevsApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("AiDevsTasks").Get<AiDevsClientConfiguration>()
                     ?? throw new InvalidOperationException($"{nameof(AiDevsClientConfiguration)} is missing");
        services.AddSingleton(config);

        services.AddHttpClient<AiDevsClient>(x => x.BaseAddress = config.BaseUrl);
    }
}