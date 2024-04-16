using AiDevs2.Tasks.ApiClients;
using AiDevs2.Tasks.Tasks.Common;
using Microsoft.Extensions.Logging;

namespace AiDevs2.Tasks.Tasks;

public class OptimalDb(AiDevsClient aiDevsClient, ILogger<AiDevsTaskBase> logger) 
    : AiDevsTaskBase("optimaldb", aiDevsClient, logger)
{
    public override async Task Run()
    {
        var task = await GetTask();
        var prompt = """
                     User
                     Your task is to extract and summarize all information about each user from delivered facts. Respond in english. Ignore not important text and keep only information that is needed to answer questions about this person.
                     I will provide you it in a JSON format and I expect the same response but each fact is shorter without losing important information. Do not change the meaning of words, always use short synonym.
                     Based on fact add new fact with your guess about person's job


                     your answer is only JSON with summarized facts per person. do not change json structure, just summarize each string in array. Do not repeat person's name in summarize of fact to save size

                     please extract only important facts to save tokens
                     
                     input json:
                     ###JSON_CONTENTS
                     """;
        await SubmitAnswer(await File.ReadAllTextAsync("Tasks/OptimalDbTask/db.json"));
    }
}