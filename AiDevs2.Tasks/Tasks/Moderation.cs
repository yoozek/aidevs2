using System.Text;
using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Moderation(AiDevsClient aiDevsClient, OpenAiClientConfiguration openAiConfig, ILogger<Moderation> logger)
    : AiDevsTaskBase("moderation", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<ModerateTaskResponse>();

        var moderationCheckResults = new List<int>();
        foreach (var inputMessage in task.Input)
        {
            var moderateResponse = await ModerateText(inputMessage);
            moderationCheckResults.Add(moderateResponse.Results.First().Flagged ? 1 : 0);
        }

        await SubmitAnswer(moderationCheckResults);
    }

    private async Task<ModerateApiResponse> ModerateText(string text)
    {
        var json = JsonSerializer.Serialize(new { input = text }, JsonSerializerOptions);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/moderations");
        requestMessage.Headers.Add("Authorization", $"Bearer {openAiConfig.ApiKey}");
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ModerateApiResponse>(content, JsonSerializerOptions)!;
    }

    private record ModerateTaskResponse(List<string> Input);

    private record ModerateApiResponse(List<ModerateApiResult> Results);

    public record ModerateApiResult(bool Flagged);
}