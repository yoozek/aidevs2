using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Tools(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<Tools> logger)
    : AiDevsTaskBase("tools", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask<ToolsTaskResponse>();

        var getIntentPrompt = $@"
Decide whether the task should be added to the ToDo list 
or to the calendar (if time is provided) and return the corresponding JSON

Today is {DateTime.Now}

Always use YYYY-MM-DD format for dates.
Always use the same language as user.

Example for ToDo list:
 - Remind me to buy milk
returns
{{ ""tool"": ""ToDo"", ""desc"": ""Buy milk"" }}

Example for Calendar:
 - Jutro mam spotkanie z Marianem
returns
{{ ""tool"": ""Calendar"", ""desc"": ""Spotkanie z Marianem"", ""date"": ""2024-04-10"" }}
";

        var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-4",
            Messages =
            {
                new ChatRequestSystemMessage(getIntentPrompt),
                new ChatRequestUserMessage(task.Question)
            }
        });
        var intentResult = response.Value.Choices[0].Message.Content;

        var intent = JsonSerializer.Deserialize<ToolAction>(intentResult, JsonSerializerOptions);
        if (intent == null)
        {
            logger.LogError($"Unable to determine action for: {task.Question}");
            return;
        }

        logger.LogInformation(task.Question);
        logger.LogInformation(intent.ToString());

        await SubmitAnswer(new
        {
            // case-sensitive names in answer
            tool = intent.Tool,
            desc = intent.Desc,
            date = intent.Date
        });
    }

    private record ToolsTaskResponse(string Question);

    private record ToolAction(string Tool, string Desc, DateOnly? Date);
}