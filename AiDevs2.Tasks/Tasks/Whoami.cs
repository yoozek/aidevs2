using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Whoami(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<Whoami> logger)
    : AiDevsTaskBase("whoami", aiDevsClient, logger)
{
    public override async Task Run()
    {
        string? personName = null;
        var hints = new List<string>();
        do
        {
            if (hints.Count > 10)
                break;

            var task = await GetTask<WhoamiTaskResponse>();
            hints.Add(task.Hint);
            var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
            {
                DeploymentName = "gpt-3.5-turbo-0125",
                Messages =
                {
                    new ChatRequestSystemMessage("""
                                                    We play in 'Who is this?' game. 
                                                    User provides a list of facts about person.
                                                    Your task is to return person's name based on provided facts.
                                                    Return person name and nothing more only if you are really sure about it.
                                                    If you are not sure to determine person based on provided facts 
                                                    or you have multiple possible answers just answer NO
                                                 """),
                    new ChatRequestUserMessage($"<subject>{string.Join(Environment.NewLine, hints)}</subject>")
                }
            });
            var personNameResponse = response.Value.Choices[0].Message.Content;
            personName = personNameResponse != "NO" ? personNameResponse : null;
        } while (personName == null);

        if (personName == null)
        {
            logger.LogInformation($"Cannot determine person name with provided hints: \n {string.Join(Environment.NewLine, hints)}");
            return;
        }

        logger.LogInformation($"The person is: {personName}");
        await SubmitAnswer(personName);
    }

    private record WhoamiTaskResponse(string Hint);
}