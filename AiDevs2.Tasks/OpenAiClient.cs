using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiDevs2.Tasks;

public static class OpenAiServiceCollectionExtensions
{
    public static void AddOpenAiService(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["OpenAi:ApiKey"] 
                     ?? throw new InvalidOperationException("Missing configuration OpenAi:ApiKey");

        services.AddSingleton(new OpenAIClient(apiKey));
    }
}