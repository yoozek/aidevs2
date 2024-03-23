using System.Text;
using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace AiDevs2.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console()
            .CreateLogger();

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder
            .AddJsonFile("appsettings.json", false, true);
        var configuration = configurationBuilder.Build();

        var serviceProvider = ConfigureServices(configuration);
        await serviceProvider.GetRequiredService<App>().Run(args);
    }

    private static ServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddSerilog());

        services.AddAiDevsApiClient(configuration);
        services.AddOpenAiApiClient(configuration);

        services.AddSingleton<App>();
        services.Scan(scan => scan
            .FromAssemblyOf<AiDevsTaskBase>()
            .AddClasses(classes => classes.AssignableTo<AiDevsTaskBase>())
            .AsSelf()
            .WithScopedLifetime());

        return services.BuildServiceProvider();
    }
}