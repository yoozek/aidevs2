using AiDevs3.Poligon.Tasks.Common;
using OpenAI.Chat;

namespace AiDevs3.Poligon.Tasks;

public class Censorship(AiDevsConfig aiDevsConfig, OpenAiConfig openAiConfig) : PoligonTask(aiDevsConfig)
{
    protected override string Name => "CENZURA";
    private readonly string _censorshipFileUrl = $"https://centrala.ag3nts.org/data/{aiDevsConfig.ApiKey}/cenzura.txt";


    protected internal override async Task Run()
    {
        using var httpClient = new HttpClient();
        string inputContent = await httpClient.GetStringAsync(_censorshipFileUrl);

        var prompt = @$"You are a data privacy expert. Your task is to identify and replace sensitive information with the word 'CENZURA' while keeping all other text, punctuation, and formatting exactly the same.
    Replace the following text between <text> and </text> tags with 'CENZURA':
    1. Full names (first name + last name)
    2. Street names with numbers
    3. City names
    4. Age (including when mentioned with word 'lat')
    Important rules:
    - Replace ONLY the sensitive information
    - Keep all punctuation marks (periods, commas, etc.) exactly as they are
    - Maintain all spacing and formatting
    - Do not add or remove any characters
    - Do not modify any other parts of the text
    Output the censored text only, without any explanations or additional text. Do not include <text> or </text> in response
<text>{inputContent}</text>";

        ChatClient client = new(model: "gpt-4o", apiKey: openAiConfig.ApiKey);

        ChatCompletion completion = await client.CompleteChatAsync(prompt);

        var response = await SendAnswer(completion.Content[0].Text);
    }
}