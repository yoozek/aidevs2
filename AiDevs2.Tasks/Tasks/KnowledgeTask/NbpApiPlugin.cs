using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Refit;

namespace AiDevs2.Tasks.Tasks.KnowledgeTask;

[Description("Plugin for retrieving currency data from National Bank of Poland. All exchange rates are in PLN.")]
public class NbpApiPlugin(INbpApi api)
{
    [KernelFunction]
    [Description("Gets the list of all exchange rates in pair with PLN")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRates()
    {
        var exchangeRates = await api.GetExchangeRates("a");
        return JsonSerializer.Serialize(exchangeRates);
    }

    [KernelFunction]
    [Description("Gets the list of all exchange rates in pair with PLN")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRateForCurrency([Description("The Code of currency")] string currencyCode)
    {
        var exchangeRates = await api.GetExchangeRateForCurrency("a", currencyCode);
        return JsonSerializer.Serialize(exchangeRates);
    }
}

public interface INbpApi
{
    [Get("/api/exchangerates/tables/{table}")]
    Task<dynamic> GetExchangeRates(string table, [AliasAs("format")] string format = "json");


    [Get("/api/exchangerates/rates/{table}/{code}")]
    Task<dynamic> GetExchangeRateForCurrency(string table, string code, [AliasAs("format")] string format = "json");
}

public static class NbpApiPluginKernelBuilderExtensions
{
    public static IKernelBuilder AddNbpApiPlugin(this IKernelBuilder builder)
    {
        builder.Plugins.AddFromType<NbpApiPlugin>();
        builder.Plugins.Services.AddRefitClient<INbpApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("http://api.nbp.pl");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

        return builder;
    }
}