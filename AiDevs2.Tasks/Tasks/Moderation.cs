using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Moderation : AiDevsTaskBase
{
    private readonly ILogger<Moderation> _logger;

    public Moderation(AiDevsService aiDevsService, ILogger<Moderation> logger) 
        : base("moderation", aiDevsService, logger)
    {
        _logger = logger;
    }

    public override async Task Run()
    {
        var task = await GetTask();
        _logger.LogInformation(task);
    }
}