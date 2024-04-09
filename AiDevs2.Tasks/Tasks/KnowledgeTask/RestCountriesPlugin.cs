using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Refit;

namespace AiDevs2.Tasks.Tasks.KnowledgeTask;

[Description("Plugin for retrieving country data like population or area.")]
public class RestCountriesPlugin(IRestCountriesApi api)
{
    [KernelFunction]
    [Description("Get information about the country")]
    [return: Description("Information about the country")]
    public async Task<string> GetByCountryName(string name)
    {
        var response = await api.GetByCountryName(name);
        return JsonSerializer.Serialize(response);
    }
}

public interface IRestCountriesApi
{
    [Get("/v2/name/{name}")]
    Task<List<dynamic>> GetByCountryName(string name);
}

public static class RestCountriesPluginKernelBuilderExtensions
{
    public static IKernelBuilder AddRestCountriesPlugin(this IKernelBuilder builder)
    {
        builder.Plugins.AddFromType<RestCountriesPlugin>();
        builder.Plugins.Services.AddRefitClient<IRestCountriesApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://restcountries.com");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

        return builder;
    }
}