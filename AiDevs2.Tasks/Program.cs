using AiDevs2.Tasks.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Reflection;

namespace AiDevs2.Tasks;
//TODO: Upgrade to .net 8
class Program
{
    static async Task Main(string[] args)
    {
        var taskName = GetTaskName(args);

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder
            .AddJsonFile("appsettings.json", false);

        var configuration = configurationBuilder.Build();

        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddAiDevsApiClient();
        services.AddScoped<HelloApi>(); //TODO: Scrutor to register all tasks

        var container = services.BuildServiceProvider();

        await RunTask(taskName, container);
    }

    private static string GetTaskName(string[] args)
    {
        if (args.Any())
            return args[0];

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Wybierz zadanie:")
                .AddChoices(Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(AiDevsTaskBase)))
                    .Select(type => type.Name)));
    }

    private static async Task RunTask(string taskName, ServiceProvider container)
    {
        var taskType = Type.GetType($"AiDevs2.Tasks.Tasks.{taskName}");
        if (container.GetService(taskType ?? throw new InvalidOperationException()) is AiDevsTaskBase taskInstance)
            await taskInstance.Run();
        else
            throw new InvalidOperationException($"Nie można uruchomić '{taskName}'.");
    }
}