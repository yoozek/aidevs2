using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AiDevs2.Tasks.Tasks;

public class Blogger(AiDevsService aiDevsService, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("blogger", aiDevsService, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        var taskResponse = JsonConvert.DeserializeObject<BloggerTaskResponse>(task)!;

        var generatedParagraphs = new List<string>();
        foreach (var subject in taskResponse.Blog)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages =
                {
                    new ChatRequestSystemMessage(
                        "Napisz wpis na bloga (w języku polskim) na temat przyrządzania pizzy Margherity. Użytkownik podaje temat a twoim zadaniem jest zwrócić rozdział."),
                    new ChatRequestUserMessage(subject)
                }
            };

            var response = await openAiClient.GetChatCompletionsAsync(chatCompletionsOptions);
            generatedParagraphs.Add(response.Value.Choices[0].Message.Content);
        }

        await SubmitAnswer(generatedParagraphs);
    }
}

public record BloggerTaskResponse(List<string> Blog);