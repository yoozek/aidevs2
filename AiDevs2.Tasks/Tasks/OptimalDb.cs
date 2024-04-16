using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class OptimalDb(AiDevsClient aiDevsClient, ILogger<AiDevsTaskBase> logger) 
    : AiDevsTaskBase("optimaldb", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();

        await SubmitAnswer(await File.ReadAllTextAsync("Tasks/OptimalDbTask/db.json"));
    }
}