using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace AiDevs2.Tasks.Tasks.Common;

public abstract class AiDevsTaskBase(string taskName, AiDevsClient aiDevsClient, ILogger<AiDevsTaskBase> logger)
    : IDisposable
{
    protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private string? _token;

    protected AsyncRetryPolicy RetryPolicy = Policy
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    protected string TaskName = taskName;

    public void Dispose()
    {
        _token = null;
    }

    public abstract Task Run();

    protected async Task<T> GetTask<T>(Dictionary<string, string>? formData = null)
    {
        return JsonSerializer.Deserialize<T>(await GetTask(formData), JsonSerializerOptions)!;
    }

    protected async Task<string> GetTask(Dictionary<string, string>? formData = null)
    {
        return await RetryPolicy.ExecuteAsync(async () =>
        {
            logger.LogInformation($"Pobieranie zadania '{TaskName}'");
            _token ??= await aiDevsClient.GetAuthenticationToken(TaskName);
            var task = await aiDevsClient.GetTask(_token, formData);
            logger.LogDebug(task);

            return task;
        });
    }

    protected async Task<string> SubmitAnswer(object answer)
    {
        if (_token == null) throw new InvalidOperationException("Najpierw pobierz zadanie");

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            logger.LogInformation($"Wysyłanie odpowiedzi {answer}");
            var response = await aiDevsClient.SubmitAnswer(_token, JsonSerializer.Serialize(new { answer }));
            logger.LogInformation(response);

            return response;
        });
    }

    protected async Task<string?> GetHint()
    {
        logger.LogInformation($"Pobieranie podpowiedzi '{TaskName}'");
        var hint = await aiDevsClient.GetHint(TaskName);
        logger.LogDebug(hint);

        return hint;
    }


    protected async Task<Stream> GetHttpFileStream(string fileUri)
    {
        using var httpClient = new HttpClient();
        using var httpResponse = await httpClient.GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead);
        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadAsStreamAsync();
    }

    protected async Task<string> GetHttpFileText(string fileUri)
    {
        using var httpClient = new HttpClient();
        using var httpResponse = await httpClient.GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead);
        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadAsStringAsync();
    }
}