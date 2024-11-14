using AiDevs3.Poligon.Tasks.Common;
using OpenAI.Chat;

namespace AiDevs3.Poligon.Tasks.Summarize;

public class Summarize(AiDevsConfig aiDevsConfig, OpenAiConfig openAiConfig) : PoligonTask(aiDevsConfig)
{
    protected override string Name { get; }
    protected internal override async Task Run()
    {
        var inputFile = @"C:\Users\jozwi\Downloads\summary\S02E03-1731491327.md";
        var ouputDirectory = @"C:\Users\jozwi\Downloads\summary\";
        var client = new ChatClient("gpt-4o", openAiConfig.ApiKey);

        var service = new ContentProcessor(client, ouputDirectory);
        await service.ProcessContent(inputFile, "title");
    }
}