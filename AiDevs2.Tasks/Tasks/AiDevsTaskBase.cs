﻿using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public abstract class AiDevsTaskBase(string taskName, AiDevsClient aiDevsClient, ILogger<AiDevsTaskBase> logger)
{
    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private string? _token;
    protected string TaskName = taskName;

    public abstract Task Run();

    protected async Task<T> GetTask<T>()
    {
        return JsonSerializer.Deserialize<T>(await GetTask(), JsonSerializerOptions)!;
    }

    protected async Task<string> GetTask(Dictionary<string, string>? formData = null)
    {
        logger.LogInformation($"Pobieranie zadania '{TaskName}'");
        _token = await aiDevsClient.GetAuthenticationToken(TaskName);
        var task = await aiDevsClient.GetTask(_token, formData);
        logger.LogDebug(task);

        return task;
    }

    protected async Task<string> SubmitAnswer(object answer)
    {
        if (_token == null) throw new InvalidOperationException("Najpierw pobierz zadanie");

        logger.LogInformation($"Wysyłanie odpowiedzi {answer}");
        var response = await aiDevsClient.SubmitAnswer(_token, JsonSerializer.Serialize(new { answer }));
        logger.LogInformation(response);

        return response;
    }
}