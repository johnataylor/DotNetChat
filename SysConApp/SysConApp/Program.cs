
using SysConApp;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;
using Azure.Identity;
using Azure.Core;

async Task Test0()
{
    try
    {
        var agent = await CreateAgentAsync();

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

async Task<Agent> CreateAgentAsync()
{
    string apiKey = File.ReadAllText(@"C:\keys\openai.txt");

    var cred = new DefaultAzureCredential(
        new DefaultAzureCredentialOptions
        {
            ExcludeVisualStudioCredential = true,
            ExcludeInteractiveBrowserCredential = false,
            InteractiveBrowserCredentialClientId = "51f81489-12ee-4a9e-aaae-a2591f45987d",
        });
    var accessToken = await cred.GetTokenAsync(new Azure.Core.TokenRequestContext(new[] { "https://aurorabapenvc1989.crm10.dynamics.com/user_impersonation" }));

    return new Agent(apiKey, FetchStructuredDataAsync, FetchUnstructuredDataAsync, accessToken);
}

static async Task<string> FetchStructuredDataAsync(string queryText, AccessToken accessToken, bool verbose)
{
    try
    {
        using HttpClient client = new();
        HttpRequestHeaders headers = client.DefaultRequestHeaders;
        headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        headers.Add("Host", "aurorabapenvc1989.crm10.dynamics.com");
        headers.Add("Authorization", $"Bearer {accessToken.Token}");

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

        var sql = obj?["query"]?["tSql"]?.GetValue<string>() ?? string.Empty;

        if (verbose)
        {
            ConsoleLogger.LogGeneratedSql(sql);
        }

        var summary = obj?["queryResult"]?["summary"]?.GetValue<string>() ?? string.Empty;

        return summary;
    }
    catch (Exception ex)
    {
        await Console.Out.WriteLineAsync(ex.Message);
        if (ex.InnerException != null)
        {
            await Console.Out.WriteLineAsync(ex.InnerException.Message);
        }
        throw;
    }
}

static async Task<string> FetchUnstructuredDataAsync(string queryText, AccessToken accessToken, bool verbose)
{
    try
    {
        using HttpClient client = new();
        HttpRequestHeaders headers = client.DefaultRequestHeaders;
        headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        headers.Add("Host", "aurorabapenvc1989.crm10.dynamics.com");
        headers.Add("Authorization", $"Bearer {accessToken.Token}");

        var requestBody = new
        {
            searchText = queryText,
            entityParameters = new[] { new { name = "msdyn_kbattachment" } },
            totalResultCount = true,
            top = 1
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync("https://aurorabapenvc1989.crm10.dynamics.com/api/copilot/v1.0/QueryTextContext", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        return responseContent;
    }
    catch (Exception ex)
    {
        await Console.Out.WriteLineAsync(ex.Message);
        if (ex.InnerException != null)
        {
            await Console.Out.WriteLineAsync(ex.InnerException.Message);
        }
        throw;
    }
}

