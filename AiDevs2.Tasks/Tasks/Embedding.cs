using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Embedding(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("embedding", aiDevsClient, logger)
{
    public override async Task Run()
    {
        await GetTask();

        var response = openAiClient.GetEmbeddingsAsync(new EmbeddingsOptions
        {
            DeploymentName = "text-embedding-ada-002",
            Input = { "Hawaiian pizza" }
        });

        var vectors = response.Result.Value.Data.Select(x => x.Embedding).ToArray();
        await SubmitAnswer(vectors[0]);
    }
}