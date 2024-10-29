namespace AiDevs3.Poligon.Tasks.Common;

public abstract class PoligonTask
{
    public abstract string Name { get; }
    public abstract Task Run();
}