using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using AiDevs2.Tasks.Tasks.KnowledgeTask;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AiDevs2.Tasks.Tasks;

public class AKnowledge(AiDevsClient aiDevsClient, OpenAiClientConfiguration openAiConfig, ILogger<HelloApi> logger)
    : AiDevsTaskBase("knowledge", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask< KnowledgeTaskResponse>();

        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo",
            openAiConfig.ApiKey);

        //TODO: Add plugins
        // [DONE] 1. Exchange rates plugin Q: podaj aktualny kurs EURO
        // 2. World Countries data Q: podaj populację Francji
        // 3. General knowledge (gpt) Q: kto napisał Romeo i Julia?
        builder.Plugins.AddFromType<NbpApiPlugin>();

        var kernel = builder.Build();
        // TODO: Moderate input 

        //DEBUG
        task = new KnowledgeTaskResponse("podaj aktualny kurs EURO");

        var questionPrompt = kernel.CreateFunctionFromPrompt(task.Question, new OpenAIPromptExecutionSettings { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions});

        var result = await kernel.InvokeAsync(questionPrompt);


    }

    private record KnowledgeTaskResponse(string Question);
}