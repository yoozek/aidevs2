using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AiDevs2.Tasks.Tasks;

public class HelloApi(AiDevsService aiDevsService, ILogger<HelloApi> logger)
    : AiDevsTaskBase("helloapi", aiDevsService, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();

        var jsonObj = JsonConvert.DeserializeObject<dynamic>(task)!;
        await SubmitAnswer(jsonObj.cookie.ToString());
    }
}