
using SysConApp;
using System.Reflection.Metadata.Ecma335;

async Task Test0()
{
    try
    {
        var agent = CreateAgent();
        while (true)
        {
            Console.Write("user: ");
            var userInput = Console.ReadLine() ?? string.Empty;

            var (quit, command) = ProcessCommand(agent, userInput);

            if (quit)
            {
                break;
            }

            if (command)
            {
                continue;
            }

            var assistantResponse = await agent.RunAsync(userInput);
            Console.WriteLine($"assistant: {assistantResponse}");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("exception...");
        Console.WriteLine(e.Message);
    }
}

await Test0();

static (bool, bool) ProcessCommand(Agent agent, string userInput)
{
    if (userInput == "bye")
    {
        return (true, true);
    }

    switch (userInput.ToLower().Trim())
    {
        case "":
            return (false, true);

        case "clear-transcript":
            agent.Transcript.Clear();
            return (false, true);

        case "clear-context":
            agent.Context.Clear();
            return (false, true);

        case "dump-transcript":
            foreach (var line in agent.Transcript)
            {
                Console.WriteLine($"  {line}");
            }
            return (false, true);

        case "dump-context":
            foreach (var line in agent.Context)
            {
                Console.WriteLine($"  {line}");
            }
            return (false, true);
    }

    if (userInput.StartsWith("context:"))
    {
        agent.Context.Add(userInput.Split(':')[1]);
        return (false, true);
    }

    return (false, false);
}

static Agent CreateAgent()
{
    string apiKey = File.ReadAllText(@"C:\keys\openai.txt");
    return new Agent(apiKey);
}

