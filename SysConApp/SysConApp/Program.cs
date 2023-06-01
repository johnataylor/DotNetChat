
using SysConApp;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;

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

        case "verbose":
            agent.Verbose = !agent.Verbose;
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
    return new Agent(apiKey, AttemptToFetchMoreDataAsync);
}

static async Task<string> AttemptToFetchMoreDataAsync(string queryText)
{
    /*
    var cert = File.ReadAllText(@"..\..\..\ck.txt");

    using HttpClient client = new();
    HttpRequestHeaders headers = client.DefaultRequestHeaders;
    headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    headers.Add("Host", "aurorabapenvc1989.crm10.dynamics.com");
    headers.Add("Authorization", $"Bearer {cert}");

    var requestBody = new
    {
        queryText = queryText,
        //entityParameters = new[] { new { name = "msdyn_workorderproduct" }, new { name = "msdyn_workorder" }, new { name = "msdyn_workorderservicetask" }, new { name = "msdyn_priority" } }
    };

    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    HttpResponseMessage response = await client.PostAsync("https://aurorabapenvc1989.crm10.dynamics.com/api/copilot/v1.0/QueryStructuredData", content);

    var responseContent = await response.Content.ReadAsStringAsync();

    var obj = JsonNode.Parse(responseContent);

    var summary = obj?["queryResult"]?["summary"]?.GetValue<string>() ?? string.Empty;

    return summary;
    */

    // mock results

    switch (queryText)
    {
        case "what is the summary of work order 00052":
            return "The summary of work order 00052 is Install car tires.";

        case "when was work order 00049 created":
            return "00049 was created on 5/13/2023 7:15PM.";

        case "give me all completed work orders":
            return "The completed work orders are 00005, 00024, 00036, 00042, 00044, 00048.";

        case "what is the incident type for work order 00004":
            return "the incident type is Thermostat is broken";

        default:
            return string.Empty;
    }
}
