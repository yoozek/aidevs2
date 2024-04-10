using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Gnome(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<Gnome> logger)
    : AiDevsTaskBase("gnome", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<GnomeTaskResponse>();

        var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-4-turbo",
            Messages =
            {
                new ChatRequestSystemMessage("""
                                             Your task is to identify what's on the image.
                                             If this is gnome wearing a hat, return only the color of the hat in polish
                                             for example czerwony
                                             If it is not a gnome or it has no hat, return only word error
                                             """),
                new ChatRequestUserMessage(new ChatMessageImageContentItem(task.Url))
            }
        });

        var answer = response.Value.Choices[0].Message.Content;
        logger.LogInformation($"The answer is: '{answer}'");

        await SubmitAnswer(answer);
    }

    private record GnomeTaskResponse(Uri Url);
}