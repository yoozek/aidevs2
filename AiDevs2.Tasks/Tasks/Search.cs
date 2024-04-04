using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace AiDevs2.Tasks.Tasks;

public class Search(AiDevsClient aiDevsClient, ILogger<HelloApi> logger, OpenAiClientConfiguration openAiConfig)
    : AiDevsTaskBase("search", aiDevsClient, logger)
{
    private readonly string _sourceUrl = "https://unknow.news/archiwum_aidevs.json";

    public override async Task Run()
    {
        var task = await GetTask<SearchTaskResponse>();

        var memory = new KernelMemoryBuilder()
            .WithOpenAI(new OpenAIConfig
            {
                APIKey = openAiConfig.ApiKey,
                EmbeddingModel = "text-embedding-3-large",
                EmbeddingModelMaxTokenTotal = 8191,
                MaxRetries = 3,
                TextModel = "gpt-4",
                TextModelMaxTokenTotal = 4096
            })
            .WithQdrantMemoryDb("http://localhost:6333/")
            .Build<MemoryServerless>();

        var fileName = _sourceUrl.Split('/').Last();

        var importData = false;
        if (importData)
            await ImportText(memory, fileName);

        logger.LogInformation($"Pytanie: {task.Question}");
        var response = await memory.AskAsync(
            $"Odpowiedz na pytanie zwracając tylko URL i nic więcej <pytanie>{task.Question}</pytanie>", fileName);
        logger.LogInformation($"Powiązany link: {response.Result}");
        await SubmitAnswer(response.Result);
    }

    private async Task ImportText(MemoryServerless memory, string fileName)
    {
        var jsonString = await GetHttpFileText(_sourceUrl);
        var links = JsonSerializer.Deserialize<List<BookmarkEntry>>(jsonString, JsonSerializerOptions);
        if (links != null)
            foreach (var link in links)
            {
                await memory.ImportTextAsync($"<url>{link.Url}</url>\n {link.Info}", index: fileName, tags: new TagCollection
                {
                    {"title", link.Title},
                    {"url", link.Url},
                    {"date", link.Date.ToString()}
                });
            }
    }

    private record SearchTaskResponse(string Question);

    private record BookmarkEntry(string Title, string Url, string Info, DateOnly Date);
}