using AiDevs3.Poligon.Tasks.Common;
using Microsoft.Extensions.DependencyInjection;

namespace AiDevs3.Poligon;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        
        var app = serviceProvider.GetRequiredService<App>();
        await app.Run(args);
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<App>();

        services.Scan(scan => scan
            .FromAssemblyOf<PoligonTask>()
            .AddClasses(classes => classes.AssignableTo<PoligonTask>())
            .AsSelf()
            .WithTransientLifetime());

        return services.BuildServiceProvider();
    }
}