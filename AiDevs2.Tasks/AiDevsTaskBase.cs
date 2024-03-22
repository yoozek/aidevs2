using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks;

public abstract class AiDevsTaskBase
{
    protected string TaskName;
    private readonly AiDevsService _aiDevsService;
    private readonly ILogger<AiDevsTaskBase> _logger;
    private string? _token;

    protected AiDevsTaskBase(string taskName, AiDevsService aiDevsService, ILogger<AiDevsTaskBase> logger)
    {
        TaskName = taskName;
        _aiDevsService = aiDevsService;
        _logger = logger;
    }

    public abstract Task Run();

    protected async Task<string> GetTask()
    {
        _logger.LogInformation($"Pobieranie zadania '{TaskName}'");
        _token = await _aiDevsService.GetAuthenticationToken(TaskName);
        return await _aiDevsService.GetTask(_token);
    }

    protected async Task<string> SubmitAnswer(string answer)
    {
        if (_token == null)
        {
            throw new InvalidOperationException("Najpierw pobierz zadanie");
        }

        _logger.LogInformation("Wysyłanie odpowiedzi");
        return await _aiDevsService.SubmitAnswer(_token, answer);
    }
}