using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Md2Html(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<AiDevsTaskBase> logger) 
    : AiDevsTaskBase("md2html", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<Md2HtmlTaskResponse>();

        var answer = Console.ReadLine();
        logger.LogInformation($"The answer is: '{answer}'");

        await SubmitAnswer(answer);
    }

    private record Md2HtmlTaskResponse(string Input);
}