﻿using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AiDevs2.Tasks;

public class AiDevsService(HttpClient httpClient, IConfiguration configuration)
{
    private const string BaseUrl = "https://tasks.aidevs.pl";

    private readonly string _apiKey = configuration["AiDevsTasks:ApiKey"]
                                      ?? throw new InvalidOperationException("Missing configurationAiDevsTasks:ApiKey");

    public async Task<string> GetAuthenticationToken(string taskName)
    {
        var response = await httpClient.PostAsync($"{BaseUrl}/token/{taskName}",
            new StringContent(JsonSerializer.Serialize(new { apikey = _apiKey }), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content);
        return json.GetProperty("token").GetString() ?? throw new InvalidOperationException("No token in response");
    }

    public async Task<string> GetTask(string token)
    {
        var response = await httpClient.GetAsync($"{BaseUrl}/task/{token}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> SubmitAnswer(string token, string answer)
    {
        var response = await httpClient.PostAsync($"{BaseUrl}/answer/{token}",
            new StringContent(JsonSerializer.Serialize(new { answer }), Encoding.UTF8, "application/json"));

        return await response.Content.ReadAsStringAsync();
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddAiDevsApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<AiDevsService>();
    }
}