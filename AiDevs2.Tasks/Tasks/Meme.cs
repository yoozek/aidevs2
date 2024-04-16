using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Meme(
    AiDevsClient aiDevsClient,
    RenderFormClientConfig renderFormConfig,
    IRenderFormApi renderFormApi,
    ILogger<Meme> logger) : AiDevsTaskBase("meme", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<MemeTaskResponse>();

        var renderRequest = new RenderRequest
        {
            Template = renderFormConfig.TemplateId,
            Data = new Dictionary<string, object>
            {
                { "image.src", task.Image },
                { "bottom-text.text", task.Text }
            }
        };

        var response = await renderFormApi.RenderV2(renderRequest);
        logger.LogInformation($"Render Request ID: {response.RequestId}");
        logger.LogInformation($"Rendered Image URL: {response.Href}");

        await SubmitAnswer(response.Href);
    }

    private record MemeTaskResponse(string Image, string Text);
}