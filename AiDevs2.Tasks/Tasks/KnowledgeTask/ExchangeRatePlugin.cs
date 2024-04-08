namespace AiDevs2.Tasks.Tasks.KnowledgeTask;

public class ExchangeRatePlugin : IDisposable
{
    readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://api.todoist.com/rest/v2")
    };



    public void Dispose()
    {
        _client.Dispose();
    }
}