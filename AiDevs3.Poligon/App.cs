using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Reflection;
using AiDevs3.Poligon.Tasks.Common;

namespace AiDevs3.Poligon;

public class App(IServiceScopeFactory scopeFactory)
{
    private const string ExitMessage = "Zakończ program";

    public async Task Run(string[] args)
    {
        using var scope = scopeFactory.CreateScope();
        do
        {
            var taskName = GetTaskName(args);
            if (taskName == ExitMessage) return;
            await RunTask(taskName, scope);
        } while (true);
    }

    private static string GetTaskName(string[] args)
    {
        if (args.Any())
            return args[0];

        var taskNames = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false }
                && type.IsSubclassOf(typeof(PoligonTask)))
            .Select(type => $"{type.Name}");

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Wybierz zadanie:")
                .AddChoices([.. taskNames, ExitMessage]));
    }

    private static async Task RunTask(string taskName, IServiceScope container)
    {
        var taskType = Type.GetType($"AiDevs3.Poligon.Tasks.{taskName}");
        if (container.ServiceProvider.GetService(taskType ?? throw new InvalidOperationException()) is PoligonTask
            taskInstance)
            await taskInstance.Run();
        else
            throw new InvalidOperationException($"Nie można uruchomić '{taskName}'.");
    }
}