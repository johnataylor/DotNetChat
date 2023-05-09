namespace SysConApp
{
    public class Agent
    {
        private readonly List<string> _transcript = new List<string>();
        private readonly DialogEngine _dialogEngine;

        public Agent(string apiKey)
        {
            _dialogEngine = new DialogEngine(apiKey);
        }

        public async Task<string> RunAsync(string userInput, string context)
        {
            _transcript.Add(userInput);
            await _dialogEngine.RunAsync(_transcript, context);
            return _transcript.Last();
        }
    }
}
