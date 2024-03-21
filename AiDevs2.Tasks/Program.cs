using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiDevs2.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder
            .AddJsonFile("appsettings.json", false);

        var configuration = configurationBuilder.Build();

        IServiceCollection services = new ServiceCollection();
        services.AddAiDevsApiClient();
        services.AddSingleton<IConfiguration>(configuration);
        var container = services.BuildServiceProvider();
        
        var client = container.GetRequiredService<AiDevsApiClient>();
        var taskName = "helloapi";
        var token = await client.GetAuthenticationToken(taskName);
        var task = await client.GetTask(token);
        Console.WriteLine(task);
    }

}