using AiDevs2.Tasks.ApiClients;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class HelloApi(AiDevsClient aiDevsClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("helloapi", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<HelloApiResponse>();
        await SubmitAnswer(task.Cookie);
    }

    private record HelloApiResponse(string Cookie);
}