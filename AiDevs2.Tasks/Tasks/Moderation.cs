using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        var answer = JsonSerializer.Serialize(new { answer = moderationCheckResults });
        await SubmitAnswer(answer);
    }

    private async Task<ModerateApiResponse> ModerateText(string text)
    {
        var apiKey = configuration["OpenAi:ApiKey"]
                     ?? throw new InvalidOperationException("Missing configuration OpenAi:ApiKey");

        // Convert your data to JSON
        var json = JsonConvert.SerializeObject(new
        {
            input = text
        });

        // Set up the request
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/moderations");
        requestMessage.Headers.Add("Authorization", $"Bearer {apiKey}");
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

        // Send the request
        using var client = new HttpClient();
        var response = await client.SendAsync(requestMessage);

        // Ensure we got a successful response
        response.EnsureSuccessStatusCode();

        // Read the response content as a string
        var content = await response.Content.ReadAsStringAsync();
        var responseResult = JsonConvert.DeserializeObject<ModerateApiResponse>(content)!;

        return responseResult;
    }

    private record ModerateTaskResponse(List<string> Input);

    private record ModerateApiResponse(List<ModerateApiResult> Results);

    public record ModerateApiResult(bool Flagged);
}