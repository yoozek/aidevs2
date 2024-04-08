using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using AiDevs2.Tasks.Tasks.KnowledgeTask;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AiDevs2.Tasks.Tasks;

public class Knowledge
    : AiDevsTaskBase
{
    private readonly ILogger<Knowledge> _logger;
    private readonly Kernel _kernel;

    private readonly KernelFunction _questionPrompt;
    private readonly string _questionPromptText =
        """
        Odpowiedz na pytanie. 
        Jeśli odpowiedź jest wartością liczbową lub walutową, zwróć tylko liczbę lub walutę.
        
        Pytanie:
        {{$question}}
        """;

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

        _questionPrompt = _kernel.CreateFunctionFromPrompt(_questionPromptText,
            new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            });
    }

    public override async Task Run()
    {
        var task = await GetTask< KnowledgeTaskResponse>();

        _logger.LogInformation($"Question: {task.Question}");

        var answer = await _kernel.InvokeAsync(_questionPrompt, new KernelArguments
        {
            { "question", task.Question}
        });

        _logger.LogInformation($"Answer: {answer}");

        await SubmitAnswer(answer.ToString());
    }

    private record KnowledgeTaskResponse(string Question);
}