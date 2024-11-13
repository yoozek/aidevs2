using AiDevs3.Poligon.Tasks.Common;
using System.Text.Json;
using OpenAI.Images;
using OpenAI.Chat;

namespace AiDevs3.Poligon.Tasks.RobotId;

public class RobotId(AiDevsConfig aiDevsConfig, OpenAiConfig openAiConfig) : PoligonTask(aiDevsConfig)
{
    private readonly AiDevsConfig _aiDevsConfig = aiDevsConfig;
    protected override string Name => "robotid";
    protected internal override async Task Run()
    {
        var url = $"https://centrala.ag3nts.org/data/{_aiDevsConfig.ApiKey}/robotid.json";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(url);
        var robotData = JsonSerializer.Deserialize<RobotIdResponse>(response);

        ImageClient client = new("dall-e-3", openAiConfig.ApiKey);
        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1024xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Uri
        };
        GeneratedImage image = await client.GenerateImageAsync(robotData?.description, options);
        
        var result = await SendAnswer(image.ImageUri.AbsoluteUri);
        Console.WriteLine(result);
    }
}

public record RobotIdResponse(string description);