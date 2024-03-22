using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AiDevs2.Tasks.Tasks;

public class HelloApi(AiDevsService aiDevsService, ILogger<HelloApi> logger)
    : AiDevsTaskBase("helloapi", aiDevsService, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        Console.WriteLine(task);

        dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(task)!;
        var response = await SubmitAnswer(jsonObj.cookie.ToString());
        Console.WriteLine(response);
    }
}