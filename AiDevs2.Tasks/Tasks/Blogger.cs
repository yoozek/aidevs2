using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Blogger(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("blogger", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        var taskResponse = JsonSerializer.Deserialize<BloggerTaskResponse>(task, JsonSerializerOptions)!;

        var generatedParagraphs = new List<string>();
        foreach (var subject in taskResponse.Blog)
        {
            logger.LogInformation($"OpenAI generuje '{subject}'");
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages =
                {
                    new ChatRequestSystemMessage(
                        "Napisz wpis na bloga (w języku polskim) na temat przyrządzania pizzy Margherity. Użytkownik podaje temat a twoim zadaniem jest zwrócić rozdział (10 zdań)."),
                    new ChatRequestUserMessage(subject)
                }
            };

            var response = await openAiClient.GetChatCompletionsAsync(chatCompletionsOptions);
            generatedParagraphs.Add(response.Value.Choices[0].Message.Content);
        }

        logger.LogInformation(string.Join(Environment.NewLine, generatedParagraphs));

        await SubmitAnswer(generatedParagraphs);
    }

    private record BloggerTaskResponse(int Code, string Msg, List<string> Blog);
}