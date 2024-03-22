using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Moderation(AiDevsService aiDevsService, ILogger<Moderation> logger)
    : AiDevsTaskBase("moderation", aiDevsService, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        logger.LogInformation(task);
    }
}