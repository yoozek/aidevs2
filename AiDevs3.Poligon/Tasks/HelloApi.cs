using System.Text;
using System.Text.Json;
using AiDevs3.Poligon.Tasks.Common;

namespace AiDevs3.Poligon.Tasks;

public class HelloApi(AiDevsConfig aiDevsConfig) : PoligonTask
{
    public override string Name => "POLIGON";
    private readonly string _inputUrl = "https://poligon.aidevs.pl/dane.txt";
    private readonly string _verifyUrl = "https://poligon.aidevs.pl/verify";

    public override async Task Run()
    {
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

    private async Task<TaskResponseDto?> SendAnswer(TaskAnswerDto answer)
    {
        var jsonContent = JsonSerializer.Serialize(answer, JsonSettings.Options);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var response = await client.PostAsync(_verifyUrl, content);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TaskResponseDto>(responseBody, JsonSettings.Options);
    }

}

public record TaskAnswerDto(string Task, string Apikey, object Answer);
public record TaskResponseDto(int Code, string Message);