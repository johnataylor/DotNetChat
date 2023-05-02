using DotNetChat;

static async Task Test0()
{
    try
    {
        var agent = await CreateAgentAsync();
        await StepAsync(agent, "I am thinking about buying a copy of the Python Cookbook where is it available?");
        await StepAsync(agent, "ok that sounds good, order me a copy");
        await StepAsync(agent, "actually I changed my mind, I don't want that book after all");
    }
    catch (Exception e)
    {
        ConsoleLogger.LogException(e);
    }
}
static async Task Test1()
{
    try
    {
        var agent = await CreateAgentAsync();
        await StepAsync(agent, "I am thinking about buying a copy of the Python Cookbook where is it available?");
        await StepAsync(agent, "ok that sounds good, order me a copy");
        await StepAsync(agent, "so who actually wrote the song Because The Night for Patti Smith?");
        await StepAsync(agent, "actually I changed my mind, I don't want that book");
    }
    catch (Exception e)
    {
        ConsoleLogger.LogException(e);
    }
}

await Test0();
//await Test1();

// some helper code to simplify our tests

static async Task StepAsync(Agent agent, string userInput)
{
    await Console.Out.WriteLineAsync($"user: {userInput}");
    var response = await agent.RunAsync(userInput);
    await Console.Out.WriteLineAsync($"assistant: {response}");
}

static async Task<Agent> CreateAgentAsync()
{
    var tools = GetTools<BookTools>();
    var messageFactory = await new MessageFactoryFactory().CreateAsync();
    string apiKey = File.ReadAllText(@"C:\keys\openai.txt");
    return new Agent(tools, messageFactory, apiKey);
}

static List<Tool> GetTools<T>()
{
    var tools = new List<Tool>();
    foreach (var methodInfo in typeof(T).GetMethods())
    {
        var attribute = (ToolAttribute?)methodInfo.GetCustomAttributes(typeof(ToolAttribute), false).FirstOrDefault();
        if (attribute != null)
        {
            var tool = new Tool(attribute?.Name ?? methodInfo.Name, attribute?.Description ?? methodInfo.Name, methodInfo.CreateDelegate<Func<string, Task<string>>>());
            tools.Add(tool);
        }
    }
    return tools;
}
