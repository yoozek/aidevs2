using System.Text.Json;
using System.Text.Json.Serialization;
using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace AiDevs2.Tasks.Tasks;

public class People(AiDevsClient aiDevsClient, ILogger<HelloApi> logger, OpenAiClientConfiguration openAiConfig)
    : AiDevsTaskBase("people", aiDevsClient, logger)
{
    private readonly string _sourceUrl = "https://tasks.aidevs.pl/data/people.json";
    private readonly bool _importData = false;

    public override async Task Run()
    {
        var task = await GetTask<PeopleTaskResponse>();

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
            await ImportPeople(memory, indexName);

        logger.LogInformation($"Pytanie: {task.Question}");

        var response = await memory.AskAsync(task.Question, indexName);
        logger.LogInformation($"Odpowiedź: {response.Result}");

        await SubmitAnswer(response.Result);
    }

    private async Task ImportPeople(MemoryServerless memory, string fileName)
    {
        var jsonString = await GetHttpFileText(_sourceUrl);
        var people = JsonSerializer.Deserialize<List<PersonEntry>>(jsonString, JsonSerializerOptions);
        if (people != null)
            foreach (var person in people)
            {
                var jsonDocument = JsonSerializer.Serialize(person, JsonSerializerOptions);
                await memory.ImportTextAsync(jsonDocument, index: fileName);
            }
    }

    private record PeopleTaskResponse(string Question);

    public class PersonEntry(string imie, string nazwisko, int wiek, string oMnie, string ulubionaPostacZKapitanaBomby, string ulubionySerial, string ulubionyFilm, string ulubionyKolor)
    {
        [JsonPropertyName("imie")]
        public string Imie { get; init; } = imie;

        [JsonPropertyName("nazwisko")]
        public string Nazwisko { get; init; } = nazwisko;

        [JsonPropertyName("wiek")]
        public int Wiek { get; init; } = wiek;

        [JsonPropertyName("o_mnie")]
        public string OMnie { get; init; } = oMnie;

        [JsonPropertyName("ulubiona_postac_z_kapitana_bomby")]
        public string UlubionaPostacZKapitanaBomby { get; init; } = ulubionaPostacZKapitanaBomby;

        [JsonPropertyName("ulubiony_serial")]
        public string UlubionySerial { get; init; } = ulubionySerial;

        [JsonPropertyName("ulubiony_film")]
        public string UlubionyFilm { get; init; } = ulubionyFilm;

        [JsonPropertyName("ulubiony_kolor")]
        public string UlubionyKolor { get; init; } = ulubionyKolor;
    }
}