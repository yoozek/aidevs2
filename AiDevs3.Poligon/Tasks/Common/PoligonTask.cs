using System.Text.Json;
using System.Text;

namespace AiDevs3.Poligon.Tasks.Common;

public abstract class PoligonTask(AiDevsConfig aiDevsConfig)
{
    protected string ReportUrl = "https://centrala.ag3nts.org/report";
    protected abstract string Name { get; }
    protected internal abstract Task Run();

    protected async Task<TaskResponseDto?> SendAnswer(object answerObject)
    {
        var answer = new TaskAnswerDto(Name, aiDevsConfig.ApiKey, answerObject);
        var jsonContent = JsonSerializer.Serialize(answer, JsonSettings.Options);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var response = await client.PostAsync(ReportUrl, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<TaskResponseDto>(responseBody, JsonSettings.Options);
    }
}

public record TaskAnswerDto(string Task, string Apikey, object Answer);
public record TaskResponseDto(int Code, string Message);