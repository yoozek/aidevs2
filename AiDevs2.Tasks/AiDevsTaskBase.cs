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
        return await aiDevsService.GetTask(_token);
    }

    protected async Task<string> SubmitAnswer(string answer)
    {
        if (_token == null) throw new InvalidOperationException("Najpierw pobierz zadanie");

        logger.LogInformation("Wysyłanie odpowiedzi");
        return await aiDevsService.SubmitAnswer(_token, answer);
    }
}