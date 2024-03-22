namespace AiDevs2.Tasks;

public abstract class AiDevsTaskBase
{
    protected string TaskName;
    private readonly AiDevsService _aiDevsService;
    private string? _token;

    protected AiDevsTaskBase(string taskName, AiDevsService aiDevsService)
    {
        TaskName = taskName;
        _aiDevsService = aiDevsService;
    }

    public abstract Task Run();

    protected async Task<string> GetTask()
    {
        Console.WriteLine($"Pobieranie zadania '{TaskName}'...");
        _token = await _aiDevsService.GetAuthenticationToken(TaskName);
        return await _aiDevsService.GetTask(_token);
    }

    protected async Task<string> SubmitAnswer(string answer)
    {
        if (_token == null)
        {
            throw new InvalidOperationException("Najpierw pobierz zadanie");
        }

        Console.WriteLine("Wysyłanie odpowiedzi...");
        return await _aiDevsService.SubmitAnswer(_token, answer);
    }
}