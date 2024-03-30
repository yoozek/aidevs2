using System.Text.Json;
using AiDevs2.Tasks.ApiClients;
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

        // Poniżej zabawa z OpenAi
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

        var response = await openAiClient.GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-4",
            Messages = { new ChatRequestUserMessage("Could you please add user for Mark Adamson born in 1993?") },
            Tools = { addUserFunctionDefinition, removeUserFunctionDefinition },
            ToolChoice = ChatCompletionsToolChoice.Auto
        });

        foreach (var toolCall in response.Value.Choices[0].Message.ToolCalls)
        {
            logger.LogInformation($"Calling {JsonSerializer.Serialize(toolCall, JsonSerializerOptions)}");
        }
    }
}