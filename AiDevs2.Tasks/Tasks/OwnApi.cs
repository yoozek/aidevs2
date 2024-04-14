using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class OwnApi(AiDevsClient aiDevsClient, ILogger<Functions> logger) 
    : AiDevsTaskBase("ownapi", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();

        await SubmitAnswer("https://ljmikr.bieda.it/aidevs2/ownapi");
    }
}

public class OwnApiPro(AiDevsClient aiDevsClient, ILogger<Functions> logger)
    : AiDevsTaskBase("ownapipro", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        var hint = await GetHint();
        await SubmitAnswer("https://ljmikr.bieda.it/aidevs2/ownapi");
    }
}