using Newtonsoft.Json;

namespace AiDevs2.Tasks.Tasks;

public class HelloApi : AiDevsTaskBase
{
    public HelloApi(AiDevsService aiDevsService) : base("helloapi", aiDevsService)
    {
    }

    public override async Task Run()
    {
        var task = await GetTask();
        Console.WriteLine(task);

        dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(task)!;
        var response = await SubmitAnswer(jsonObj.cookie.ToString());
        Console.WriteLine(response);
    }
}