using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiDevs2.Tasks.ApiClients;

public record OpenAiClientConfiguration(Uri ApiKey);

public static class OpenAiClientServiceCollectionExtensions
{
    public static void AddOpenAiApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("OpenAi").Get<OpenAiClientConfiguration>()
                     ?? throw new InvalidOperationException($"{nameof(OpenAiClientConfiguration)} is missing");
        services.AddSingleton(config);

        services.AddSingleton(new OpenAIClient(config.ApiKey.ToString()));
    }
}