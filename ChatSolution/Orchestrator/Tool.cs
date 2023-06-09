namespace Orchestrator
{
    public record Tool(string Name, string Description, bool ReturnDirect, Func<string, Task<string>> Function);
}
