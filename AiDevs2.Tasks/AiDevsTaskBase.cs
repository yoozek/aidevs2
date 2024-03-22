using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks;

public abstract class AiDevsTaskBase(string taskName, AiDevsService aiDevsService, ILogger<AiDevsTaskBase> logger)
{
    private string? _token;
    protected string TaskName = taskName;

    public abstract Task Run();

    protected async Task<string> GetTask()
    {
        logger.LogInformation($"Pobieranie zadania '{TaskName}'");
        _token = await aiDevsService.GetAuthenticationToken(TaskName);
        var task = await aiDevsService.GetTask(_token);
        logger.LogInformation(task);

        return task;
    }

    protected async Task<string> SubmitAnswer(object answer)
    {
        if (_token == null) throw new InvalidOperationException("Najpierw pobierz zadanie");

        logger.LogInformation("Wysyłanie odpowiedzi");
        var response = await aiDevsService.SubmitAnswer(_token, answer);
        logger.LogInformation(response);

        return await aiDevsService.SubmitAnswer(_token, answer);
    }
}