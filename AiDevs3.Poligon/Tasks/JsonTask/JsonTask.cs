using System.Text.Json;
using AiDevs3.Poligon.Tasks.Common;

namespace AiDevs3.Poligon.Tasks.JsonTask;

public class JsonTask(AiDevsConfig aiDevsConfig) : PoligonTask(aiDevsConfig)
{
    protected override string Name => "JSON";

    protected internal override async Task Run()
    {
        var resultText = await File.ReadAllTextAsync(@"C:\Users\jozwi\source\repos\aidevs3\fix_json\output.json");
        var obj = JsonSerializer.Deserialize<dynamic>(resultText);
        var result = await SendAnswer(obj);
    }
}