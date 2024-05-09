using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

#pragma warning disable SKEXP0050
public class Google(AiDevsClient aiDevsClient, ILogger<AiDevsTaskBase> logger) : AiDevsTaskBase("google", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        await SubmitAnswer("https://api.jozwik.dev/aidevs2/google");
    }
}

