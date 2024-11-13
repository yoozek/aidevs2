using AiDevs3.Poligon.Tasks.Common;

namespace AiDevs3.Poligon.Tasks.Mp3Task;

public class Mp3Task(AiDevsConfig aiDevsConfig) : PoligonTask(aiDevsConfig)
{
    protected override string Name => "mp3";
    protected internal override async Task Run()
    {
        var response = await SendAnswer("Profesora Stanisława Łojasiewicza");
    }
}