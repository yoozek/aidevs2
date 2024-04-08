using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace AiDevs2.Tasks.Tasks;

public class AKnowledge(AiDevsClient aiDevsClient, OpenAiClientConfiguration openAiConfig, ILogger<HelloApi> logger)
    : AiDevsTaskBase("knowledge", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();

        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo",
            openAiConfig.ApiKey);



        //TODO: Add plugins
        // 1. Exchange rates plugin Q: podaj aktualny kurs EURO
        // 2. World Countries data Q: podaj populację Francji
        // 3. General knowledge (gpt) Q: kto napisał Romeo i Julia?

        // TODO: Moderate input 
    }
}