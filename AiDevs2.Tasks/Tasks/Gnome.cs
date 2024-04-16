using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AiDevs2.Tasks.Tasks;

public class Gnome(
    AiDevsClient aiDevsClient,
    OpenAIClient openAiClient,
    OpenAiClientConfiguration openAiConfig,
    ILogger<Gnome> logger)
    : AiDevsTaskBase("gnome", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<GnomeTaskResponse>();
        //await OpenAIClientSolution(task);

        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo",
            openAiConfig.ApiKey);
        var kernel = builder.Build();
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("What’s in this image?");
        chatHistory.AddUserMessage([
            new ImageContent(task.Url, metadata: new Dictionary<string, object?> { { "detail", "low" } })
        ]);
    }

    private async Task OpenAIClientSolution(GnomeTaskResponse task)
    {
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
                new ChatRequestUserMessage(new ChatMessageImageContentItem(task.Url, ChatMessageImageDetailLevel.Low))
            }
        });

        var answer = response.Value.Choices[0].Message.Content;
        logger.LogInformation($"The answer is: '{answer}'");

        await SubmitAnswer(answer);
    }

    private record GnomeTaskResponse(Uri Url);
}