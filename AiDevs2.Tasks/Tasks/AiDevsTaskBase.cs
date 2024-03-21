namespace AiDevs2.Tasks.Tasks;

public abstract class AiDevsTaskBase
{
    protected string TaskName;

    protected AiDevsTaskBase(string taskName)
    {
        TaskName = taskName;
    }

    public abstract Task Run();
}