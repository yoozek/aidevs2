using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using ILogger = Serilog.ILogger;

namespace AiDevs2.Tasks.ApiClients;

public record RenderFormClientConfig(string ApiKey, string TemplateId);

public static class RenderFormClientServiceCollectionExtensions
{
    public static void AddRenderFormClient(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("RenderForm").Get<RenderFormClientConfig>()
                     ?? throw new InvalidOperationException($"{nameof(RenderFormClientConfig)} is missing");
        services.AddSingleton(config);
        services.AddSingleton(RestService.For<IRenderFormApi>(new HttpClient
        {
            BaseAddress = new Uri("https://get.renderform.io/"),
            DefaultRequestHeaders =
            {
                { "X-API-KEY", config.ApiKey }
            }
        }));
    }
}

public interface IRenderFormApi
{
    [Post("/api/v2/render")]
    Task<RenderResponse> RenderV2([Body] RenderRequest renderRequest);
}

public class RenderRequest
{
    public string Template { get; set; } // Template ID
    public Dictionary<string, object> Data { get; set; } // Data to be merged into the template
    public string FileName { get; set; } // Name of the file to be returned
    public string WebhookUrl { get; set; } // Webhook URL to be called when the render is done
    public string Version { get; set; } // Cache key used for caching the rendered image
    public Dictionary<string, object> Metadata { get; set; } // Additional metadata for the webhook
    public string BatchName { get; set; } // Batch name for grouping renders
}

public class RenderResponse
{
    public string RequestId { get; set; } // Identifier for the render request
    public string Href { get; set; } // URL or a link to the rendered image
}