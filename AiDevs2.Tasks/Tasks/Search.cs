﻿using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace AiDevs2.Tasks.Tasks;

public class Search(AiDevsClient aiDevsClient, ILogger<Search> logger, OpenAiClientConfiguration openAiConfig)
    : AiDevsTaskBase("search", aiDevsClient, logger)
{
    private readonly bool _importData = false;
    private readonly string _sourceUrl = "https://unknow.news/archiwum_aidevs.json";

    public override async Task Run()
    {
        var task = await GetTask<SearchTaskResponse>();

        var memory = new KernelMemoryBuilder()
            .WithOpenAI(new OpenAIConfig
            {
                APIKey = openAiConfig.ApiKey,
                EmbeddingModel = "text-embedding-3-small",
                EmbeddingModelMaxTokenTotal = 8191,
                MaxRetries = 3,
                TextModel = "gpt-4",
                TextModelMaxTokenTotal = 4096
            })
            .WithQdrantMemoryDb("http://localhost:6333/")
            .Build<MemoryServerless>();

        var indexName = _sourceUrl.Split('/').Last();

        if (_importData)
            await ImportLinks(memory, indexName);

        logger.LogInformation($"Pytanie: {task.Question}");
        var response = await memory.AskAsync(
            $"Odpowiedz na pytanie zwracając tylko URL i nic więcej <pytanie>{task.Question}</pytanie>", indexName);
        logger.LogInformation($"Powiązany link: {response.Result}");
        await SubmitAnswer(response.Result);
    }

    private async Task ImportLinks(MemoryServerless memory, string fileName)
    {
        var jsonString = await GetHttpFileText(_sourceUrl);
        var links = JsonSerializer.Deserialize<List<BookmarkEntry>>(jsonString, JsonSerializerOptions);
        if (links != null)
            foreach (var link in links)
                await memory.ImportTextAsync($"<url>{link.Url}</url>\n {link.Info}", index: fileName,
                    tags: new TagCollection
                    {
                        { "title", link.Title },
                        { "url", link.Url },
                        { "date", link.Date.ToString() }
                    });
    }

    private record SearchTaskResponse(string Question);

    private record BookmarkEntry(string Title, string Url, string Info, DateOnly Date);
}