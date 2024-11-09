using System.Text;
using System.Text.Json;
using AiDevs3.Poligon.Tasks.Common;

namespace AiDevs3.Poligon.Tasks;

public class HelloApi(AiDevsConfig aiDevsConfig) : PoligonTask(aiDevsConfig)
{
    protected override string Name => "POLIGON";
    private readonly string _inputUrl = "https://poligon.aidevs.pl/dane.txt";
    private readonly string _verifyUrl = "https://poligon.aidevs.pl/verify";

    protected internal override async Task Run()
    {
        ReportUrl = _verifyUrl;

        var codes = await GetCodes();
        var answer = new TaskAnswerDto(Name, aiDevsConfig.ApiKey, codes);
        var response = await SendAnswer(answer);
    }

    private async Task<string[]> GetCodes()
    {
        using var client = new HttpClient();
        var content = await client.GetStringAsync(_inputUrl);
        return content.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
    }
}