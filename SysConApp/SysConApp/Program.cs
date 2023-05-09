
using SysConApp;

async Task Test0()
{
    try
    {
        var context = new List<string>();

        var agent = CreateAgent();
        while (true)
        {
            Console.Write("user: ");
            var userInput = Console.ReadLine() ?? string.Empty;

            if (userInput == "bye")
            {
                break;
            }

            if (userInput.Trim() == string.Empty)
            {
                continue;
            }

            if (userInput == "clear-transcript")
            {
                agent = CreateAgent();
                continue;
            }

            if (userInput == "clear-context")
            {
                context.Clear();
                continue;
            }

            if (userInput.StartsWith("context:"))
            {
                context.Add(userInput.Split(':')[1]);
                continue;
            }

            var contextContent = string.Join('\n', context);

            var response = await agent.RunAsync(userInput, contextContent);

            Console.WriteLine($"assistant: {response}");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("exception...");
        Console.WriteLine(e.Message);
    }
}

await Test0();

static Agent CreateAgent()
{
    string apiKey = File.ReadAllText(@"C:\keys\openai.txt");
    return new Agent(apiKey);
}

