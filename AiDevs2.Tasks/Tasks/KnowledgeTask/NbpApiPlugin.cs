using System.ComponentModel;
using Microsoft.SemanticKernel;
using System.Text.Json;
using Refit;

namespace AiDevs2.Tasks.Tasks.KnowledgeTask;

public class NbpApiPlugin
{
    private INbpApi Client => RestService.For<INbpApi>("http://api.nbp.pl");

    [KernelFunction, Description("Gets the list of all exchange rates")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRates()
    {
        var exchangeRates = await Client.GetExchangeRates("a");
        return JsonSerializer.Serialize(exchangeRates);
    }

    [KernelFunction, Description("Gets the list of all exchange rates")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRateForCurrency([Description("The Code of currency")]string currencyCode)
    {
        var exchangeRates = await Client.GetExchangeRateForCurrency("a", currencyCode);
        return JsonSerializer.Serialize(exchangeRates);
    }
}

public interface INbpApi
{
    [Get("/api/exchangerates/tables/{table}")]
    Task<List<ExchangeRate>> GetExchangeRates(string table, [AliasAs("format")] string format = "json");


    [Get("/api/exchangerates/rates/{table}/{code}")]
    Task<ExchangeRate> GetExchangeRateForCurrency(string table, string code, [AliasAs("format")] string format = "json");
}

public class ExchangeRate
{
    public string Table { get; set; }
    public string No { get; set; }
    public string EffectiveDate { get; set; }
    public List<Rate> Rates { get; set; }
}

public class Rate
{
    public string Currency { get; set; }
    public string Code { get; set; }
    public double Mid { get; set; }
}