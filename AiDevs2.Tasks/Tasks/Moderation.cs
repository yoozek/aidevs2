namespace AiDevs2.Tasks.Tasks;

public class Moderation : AiDevsTaskBase
{
    public Moderation(AiDevsService aiDevsService) : base("moderation", aiDevsService)
    {
    }

    public override async Task Run()
    {
        var task = await GetTask();
    }
}