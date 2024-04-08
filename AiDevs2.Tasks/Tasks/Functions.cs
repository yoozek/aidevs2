using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class Functions(AiDevsClient aiDevsClient, OpenAIClient openAiClient, ILogger<HelloApi> logger)
    : AiDevsTaskBase("functions", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        logger.LogInformation(task);

        var functionDefinition = new
        {
            name = "addUser",
            description = "Add user",
            parameters = new
            {
                type = "object",
                properties = new
                {
                    name = new
                    {
                        type = "string",
                        description = "User's name",
                    },
                    surname = new
                    {
                        type = "string",
                        description = "User's surname",
                    },
                    year = new
                    {
                        type = "integer",
                        description = "integer",
                    }
                }
            }
        };
        await SubmitAnswer(functionDefinition);

        // Poniżej zabawa z użyciem Azure.AI.OpenAI
        var addUserFunctionDefinition = new ChatCompletionsFunctionToolDefinition
        {
            Name = functionDefinition.name,
            Description = functionDefinition.description,
            Parameters = BinaryData.FromObjectAsJson(functionDefinition.parameters, JsonSerializerOptions)
        };

        var removeUserFunctionDefinition = new ChatCompletionsFunctionToolDefinition
        {
            Name = "removeUser",
            Description = "Removes user",
            Parameters = BinaryData.FromObjectAsJson(functionDefinition.parameters, JsonSerializerOptions)
        };

        await SendChatMessage("Czy możesz dodać użytkownika Łukasz Jóźwik urodzonego w 1993 ", 
            addUserFunctionDefinition, 
            removeUserFunctionDefinition); 

        await SendChatMessage("ユーザー Andrzej Duda を削除してください。1972 年生まれです。",
            addUserFunctionDefinition,
            removeUserFunctionDefinition);
    }

    private async Task SendChatMessage(string message, ChatCompletionsFunctionToolDefinition addUserFunctionDefinition,
        ChatCompletionsFunctionToolDefinition removeUserFunctionDefinition)
    {
        var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-4",
            Messages =
            {
                new ChatRequestUserMessage(message)
            },
            Tools = { addUserFunctionDefinition, removeUserFunctionDefinition }
        });

        var call = response.Value.Choices[0].Message.ToolCalls[0] as ChatCompletionsFunctionToolCall;
        logger.LogInformation($"Wykonuję {call?.Name}({call?.Arguments})");
    }
}