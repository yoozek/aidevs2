using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Moderation(AiDevsService aiDevsService, OpenAIClient openAiClient, ILogger<Moderation> logger)
    : AiDevsTaskBase("moderation", aiDevsService, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        logger.LogInformation(task);

        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo", 
            Messages =
            {
                new ChatRequestSystemMessage("You are a helpful assistant. You will talk like a pirate."),
                new ChatRequestUserMessage("Can you help me?"),
            }
        };
        var response = await openAiClient.GetChatCompletionsAsync(chatCompletionsOptions);
        logger.LogInformation(response.Value.Choices.First().Message.Content);
    }
}