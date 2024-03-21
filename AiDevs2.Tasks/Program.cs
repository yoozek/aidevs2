using AiDevs2.Tasks.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Reflection;

namespace AiDevs2.Tasks;

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
        {
            return args[0];
        }

        var baseType = typeof(AiDevsTaskBase);
        var assembly = Assembly.GetExecutingAssembly();
        var tasks = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType))
            .Select(type => type.Name)
            .ToList();

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Wybierz zadanie:")
                .AddChoices(tasks));
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