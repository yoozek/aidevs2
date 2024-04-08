using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Refit;

namespace AiDevs2.Tasks.Tasks.KnowledgeTask;

[Description("Plugin for retrieving country data like population or area.")]
public class RestCountriesPlugin
{
    private IRestCountriesApi Client => RestService.For<IRestCountriesApi>("https://restcountries.com");

    [KernelFunction]
    [Description("Get information about the country")]
    [return: Description("Information about the country")]
    public async Task<string> GetExchangeRates(string name)
    {
        var response = await Client.GetByCountryName(name);
        return JsonSerializer.Serialize(response);
    }
}

public interface IRestCountriesApi
{
    [Get("/v2/name/{name}")]
    Task<List<dynamic>> GetByCountryName(string name);
}