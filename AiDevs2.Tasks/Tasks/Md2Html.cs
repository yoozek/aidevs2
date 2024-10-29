using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Md2Html(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<Md2Html> logger) 
    : AiDevsTaskBase("md2html", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<Md2HtmlTaskResponse>();

        var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "ft:gpt-3.5-turbo-0125:personal:md2html:9N50ryXl",
            Messages =
            {
                new ChatRequestSystemMessage("Convert MD to HTML"),
                new ChatRequestUserMessage(task.Input)
            }
        });

        var answer = response.Value.Choices[0].Message.Content;
        logger.LogInformation($"The answer is: '{answer}'");
        await SubmitAnswer(answer);
    }

    private record Md2HtmlTaskResponse(string Input);
}