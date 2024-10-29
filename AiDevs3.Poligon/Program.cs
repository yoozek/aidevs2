using AiDevs3.Poligon.Tasks.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiDevs3.Poligon;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = BuildConfiguration();
        var serviceProvider = ConfigureServices(configuration);
        

        var app = serviceProvider.GetRequiredService<App>();
        await app.Run(args);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<Program>()
            .Build();
    }

    private static ServiceProvider ConfigureServices(IConfigurationRoot config)
    {
        var services = new ServiceCollection();

        services.AddSingleton<App>();

        var aiDevsConfig = config.GetSection(nameof(AiDevsConfig)).Get<AiDevsConfig>();
        services.AddSingleton(aiDevsConfig);

        services.Scan(scan => scan
            .FromAssemblyOf<PoligonTask>()
            .AddClasses(classes => classes.AssignableTo<PoligonTask>())
            .AsSelf()
            .WithTransientLifetime());

        return services.BuildServiceProvider();
    }
}

public record AiDevsConfig(string ApiKey);