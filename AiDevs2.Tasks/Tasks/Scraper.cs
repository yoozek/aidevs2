using AiDevs2.Tasks.ApiClients;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Polly;

namespace AiDevs2.Tasks.Tasks;

public class Scraper(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("scraper", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<ScraperTaskResponse>();
        logger.LogInformation($"Pobieranie artykułu {task.Input}");
        var articleText = await DownloadFileWithRetryAsync(task.Input);

        logger.LogInformation($"Odpowiadanie na pytanie '{task.Question}'");
        var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo",
            Messages =
            {
                new ChatRequestSystemMessage(
                    $"""
                        Return answer for the question in POLISH language, based on provided article. 
                        Maximum length for the answer is 200 characters
                        <article>{articleText}</article>
                        <question>{task.Question}</question>
                    """)
            }
        });

        var answer = response.Value.Choices[0].Message.Content;
        logger.LogInformation($"The answer is: '{answer}'");

        await SubmitAnswer(answer);
    }

    private async Task<string> DownloadFileWithRetryAsync(string url)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }

    private record ScraperTaskResponse(string Input, string Question);
}