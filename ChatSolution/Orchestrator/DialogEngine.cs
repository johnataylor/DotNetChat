using System.Text.Json;

namespace Orchestrator
{
    public class DialogEngine : IDialogEngine
    {
        private readonly IEnumerable<Tool> _tools;
        private readonly string _apiKey;
        private readonly IMessageFactoryProvider _messageFactoryProvider;

        public DialogEngine(IEnumerable<Tool> tools, string apiKey)
        {
            _tools = tools;
            _apiKey = apiKey;
            _messageFactoryProvider = new MessageFactoryProvider();
        }

        public async Task<(string newState, string assistantResponse)> RunAsync(string? oldState, string userRequest)
        {
            var messageFactory = await _messageFactoryProvider.CreateAsync();

            var toolExecutor = new ToolExecutor(_tools, messageFactory, _apiKey);

            var transcript = oldState == null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(oldState) ?? new List<string>();
            
            transcript.Add(userRequest);
            await toolExecutor.RunAsync(transcript);
            var assistantResponse = transcript.Last();
            
            var newState = JsonSerializer.Serialize(transcript);

            return (newState, assistantResponse);
        }
    }
}
