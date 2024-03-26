using AiDevs2.Tasks.ApiClients;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Blogger(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("blogger", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<BloggerTaskResponse>();

        var paragraphs = await Task.WhenAll(task.Blog.Select(GenerateParagraph).ToList());

        logger.LogInformation(string.Join(Environment.NewLine, paragraphs));

        await SubmitAnswer(paragraphs);
    }

    private async Task<string> GenerateParagraph(string subject)
    {
        logger.LogInformation($"Generowanie paragrafu dla '{subject}'");
        var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo",
            Messages =
            {
                new ChatRequestSystemMessage("""
                     Napisz wpis na bloga (w języku polskim) na temat przyrządzania pizzy Margherity.
                     Użytkownik podaje temat pomiędzy tagami <subject> i </subject>
                     Twoim zadaniem jest wygenerować tekst rozdziału dla tematu podanego przez użytkownika (10 zdań).
                     """),
                new ChatRequestUserMessage($"<subject>{subject}</subject>")
            }
        });
        return response.Value.Choices[0].Message.Content;
    }

    private record BloggerTaskResponse(List<string> Blog);
}