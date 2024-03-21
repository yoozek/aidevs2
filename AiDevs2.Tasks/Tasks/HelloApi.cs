using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace AiDevs2.Tasks.Tasks;

public class HelloApi : AiDevsTaskBase
{
    private readonly AiDevsApiClient _aiDevsApiClient;

    public HelloApi(AiDevsApiClient aiDevsApiClient) : base("helloapi")
    {
        _aiDevsApiClient = aiDevsApiClient;
    }

    public override async Task Run()
    {
        var token = await _aiDevsApiClient.GetAuthenticationToken(TaskName);
        var task = await _aiDevsApiClient.GetTask(token);
        Console.WriteLine(task);

        dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(task)!;
        _aiDevsApiClient.SubmitAnswer(token, jsonObj.cookie.ToString());
    }
}