using AiDevs2.Tasks.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Reflection;
using Serilog;

namespace AiDevs2.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console()
            .CreateLogger();

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder
            .AddJsonFile("appsettings.json", false);
        var configuration = configurationBuilder.Build();

        var provider = ConfigureServices(configuration);

        var taskName = GetTaskName(args);
        await RunTask(taskName, provider);
    }

    private static ServiceProvider ConfigureServices(IConfigurationRoot configuration)
    {
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddSerilog());
        services.AddSingleton<IConfiguration>(configuration);
        services.AddAiDevsApiClient();
        services.Scan(scan => scan
            .FromAssemblyOf<AiDevsTaskBase>()
            .AddClasses(classes => classes.AssignableTo<AiDevsTaskBase>())
            .AsSelf()
            .WithScopedLifetime());
        return services.BuildServiceProvider();
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