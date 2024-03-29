using AiDevs2.Tasks.ApiClients;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Whisper(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("whisper", aiDevsClient, logger)
{

    private string _fileUrl = "https://tasks.aidevs.pl/data/mateusz.mp3";
    public override async Task Run()
    {
        var task = await GetTask();
        using var httpClient = new HttpClient();
        using var httpResponse = await httpClient.GetAsync(_fileUrl, HttpCompletionOption.ResponseHeadersRead);
        httpResponse.EnsureSuccessStatusCode();
        
        await using var audioStream = await httpResponse.Content.ReadAsStreamAsync();
        
        
        var response = await openAiClient.GetAudioTranscriptionAsync(new AudioTranscriptionOptions()
        {
            DeploymentName = "whisper-1",
            AudioData = await BinaryData.FromStreamAsync(audioStream),
            ResponseFormat = AudioTranscriptionFormat.Verbose,
        });

        await SubmitAnswer(response.Value.Text);
    }
}