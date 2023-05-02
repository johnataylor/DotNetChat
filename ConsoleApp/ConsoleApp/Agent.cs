namespace DotNetChat
{
    public class Agent
    {
        private readonly List<string> _transcript = new List<string>();
        private readonly DialogEngine _dialogEngine;

        public Agent(List<Tool> tools, MessageFactory messageFactory, string apiKey)
        {
            _dialogEngine = new DialogEngine(tools, messageFactory, apiKey);
        }

        public async Task<string> RunAsync(string userInput)
        {
            _transcript.Add(userInput);
            await _dialogEngine.RunAsync(_transcript);
            return _transcript.Last();
        }
    }
}
