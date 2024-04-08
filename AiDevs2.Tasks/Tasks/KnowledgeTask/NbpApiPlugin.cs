using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Refit;

namespace AiDevs2.Tasks.Tasks.KnowledgeTask;

[Description("Plugin for retrieving currency data from National Bank of Poland. All exchange rates are in PLN.")]
public class NbpApiPlugin
{
    private INbpApi Client => RestService.For<INbpApi>("http://api.nbp.pl");

    [KernelFunction]
    [Description("Gets the list of all exchange rates in pair with PLN")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRates()
    {
        var exchangeRates = await Client.GetExchangeRates("a");
        return JsonSerializer.Serialize(exchangeRates);
    }

    [KernelFunction]
    [Description("Gets the list of all exchange rates in pair with PLN")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRateForCurrency([Description("The Code of currency")] string currencyCode)
    {
        var exchangeRates = await Client.GetExchangeRateForCurrency("a", currencyCode);
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