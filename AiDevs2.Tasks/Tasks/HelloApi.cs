using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class HelloApi(AiDevsClient aiDevsClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("helloapi", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        var cookie = JsonDocument.Parse(task).RootElement.GetProperty("cookie").ToString();
        await SubmitAnswer(cookie);
    }
}