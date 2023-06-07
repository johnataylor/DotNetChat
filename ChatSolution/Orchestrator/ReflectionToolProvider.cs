namespace Orchestrator
{
    public class ReflectionToolProvider<T> : IToolProvider
    {
        private static List<Tool> _tools;

        static ReflectionToolProvider()
        {
            _tools = GetToolsFromClass();
        }

        public Task<List<Tool>> GetToolsAsync()
        {
            return Task.FromResult(_tools);
        }

        private static List<Tool> GetToolsFromClass()
        {
            var tools = new List<Tool>();
            foreach (var methodInfo in typeof(T).GetMethods())
            {
                var attribute = (ToolAttribute?)methodInfo.GetCustomAttributes(typeof(ToolAttribute), false).FirstOrDefault();
                if (attribute != null)
                {
                    var name = attribute.Name ?? methodInfo.Name;
                    var description = attribute.Description ?? methodInfo.Name;
                    var returnDirect = attribute.ReturnDirect;
                    var function = methodInfo.CreateDelegate<Func<string, Task<string>>>();
                    tools.Add(new Tool(name, description, returnDirect, function));
                }
            }
            return tools;
        }
    }
}
