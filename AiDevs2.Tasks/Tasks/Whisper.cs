using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Whisper(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("whisper", aiDevsClient, logger)
{

    private readonly string _fileUrl = "https://tasks.aidevs.pl/data/mateusz.mp3";
    public override async Task Run()
    {
        await GetTask();
        await using var audioStream = await GetHttpFileStream(_fileUrl);

        var response = await openAiClient.GetAudioTranscriptionAsync(new AudioTranscriptionOptions()
        {
            DeploymentName = "whisper-1",
            AudioData = await BinaryData.FromStreamAsync(audioStream),
            ResponseFormat = AudioTranscriptionFormat.Verbose,
        });

        await SubmitAnswer(response.Value.Text);
    }
}