using System.ComponentModel;
using Microsoft.SemanticKernel;
using Refit;
using System.Text.Json;

namespace AiDevs2.Tasks.Tasks.KnowledgeTask;

public class RestCountriesPlugin
{
    private IRestCountriesApi Client => RestService.For<IRestCountriesApi>("https://restcountries.com");

    [KernelFunction, Description("Get information about the country")]
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