namespace DotNetChat
{
    public class Tool
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public bool ReturnDirect { get; init; }

        public Func<string, Task<string>> Function { get; init; }

        public Tool(string name, string description, bool returnDirect, Func<string, Task<string>> function)
        {
            Name = name;
            Description = description;
            ReturnDirect = returnDirect;
            Function = function;
        }
    }
}
