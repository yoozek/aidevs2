using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AiDevs2.Tasks.Tasks;

public class Moderation(AiDevsService aiDevsService, IConfiguration configuration, ILogger<Moderation> logger)
    : AiDevsTaskBase("moderation", aiDevsService, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();

        var taskResponse = JsonConvert.DeserializeObject<ModerateTaskResponse>(task)!;
        var moderationCheckResults = new List<int>();
        foreach (var inputMessage in taskResponse.Input)
        {
            var moderateResponse = await ModerateText(inputMessage);
            moderationCheckResults.Add(moderateResponse.Results.First().Flagged ? 1 : 0);
        }

        await SubmitAnswer(moderationCheckResults);
    }

    private async Task<ModerateApiResponse> ModerateText(string text)
    {
        var apiKey = configuration["OpenAi:ApiKey"]
                     ?? throw new InvalidOperationException("Missing configuration OpenAi:ApiKey");

        var json = JsonConvert.SerializeObject(new
        {
            input = text
        });

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/moderations");
        requestMessage.Headers.Add("Authorization", $"Bearer {apiKey}");
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<ModerateApiResponse>(content)!;
    }

    private record ModerateTaskResponse(List<string> Input);

    private record ModerateApiResponse(List<ModerateApiResult> Results);

    public record ModerateApiResult(bool Flagged);
}