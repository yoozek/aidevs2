using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using AiDevs2.Tasks.Tasks.KnowledgeTask;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AiDevs2.Tasks.Tasks;

public class Knowledge : AiDevsTaskBase
{
    private readonly ILogger<Knowledge> _logger;
    private readonly Kernel _kernel;

    public Knowledge(AiDevsClient aiDevsClient, 
        OpenAiClientConfiguration openAiConfig, 
        ILogger<Knowledge> logger) 
        : base("knowledge", aiDevsClient, logger)
    {
        _logger = logger;

        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
            "gpt-3.5-turbo",
            openAiConfig.ApiKey);

        builder.Plugins.AddFromType<NbpApiPlugin>();
        builder.Plugins.AddFromType<RestCountriesPlugin>();

        _kernel = builder.Build();
    }

    public override async Task Run()
    {
        var task = await GetTask< KnowledgeTaskResponse>();

        _logger.LogInformation($"Question: {task.Question}");

        var prompt = _kernel.CreateFunctionFromPrompt(task.Question,
            new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            });

        var answer = await _kernel.InvokeAsync(prompt, new KernelArguments
        {
            { "question", task.Question}
        });

        _logger.LogInformation($"Answer: {answer}");

        await SubmitAnswer(answer.ToString());
    }

    private record KnowledgeTaskResponse(string Question);
}