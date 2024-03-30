using AiDevs2.Tasks.ApiClients;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Rodo(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("rodo", aiDevsClient, logger)
{
    public override async Task Run()
    {
        await GetTask();

        await SubmitAnswer("Because of GDPR I please don't send me real information." +
                           "Instead use placeholders like %imie% %nazwisko% %miasto% %zawod%" +
                           "For example 'My name is %imie% %nazwisko% I live in %miasto%, I work as a %zawod%" +
                           "zawod is job role");
    }
}