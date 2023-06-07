using System.Text.Json;

namespace Orchestrator
{
    public class DialogEngine : IDialogEngine
    {
        private readonly IToolProvider _toolProvider;
        private readonly string _apiKey;
        private readonly IMessageFactoryProvider _messageFactoryProvider;

        public DialogEngine(IToolProvider toolProvider, string apiKey)
        {
            _toolProvider = toolProvider;
            _apiKey = apiKey;
            _messageFactoryProvider = new MessageFactoryProvider();
        }

        public async Task<(string newState, string assistantResponse)> RunAsync(string? oldState, string userRequest)
        {
            var tools = await _toolProvider.GetToolsAsync();
            var messageFactory = await _messageFactoryProvider.CreateAsync();

            var toolExecutor = new ToolExecutor(tools, messageFactory, _apiKey);

            var transcript = oldState == null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(oldState) ?? new List<string>();
            
            transcript.Add(userRequest);
            await toolExecutor.RunAsync(transcript);
            var assistantResponse = transcript.Last();
            
            var newState = JsonSerializer.Serialize(transcript);

            return (newState, assistantResponse);
        }
    }
}
