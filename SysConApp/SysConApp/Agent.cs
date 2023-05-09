namespace SysConApp
{
    public class Agent
    {
        private readonly DialogEngine _dialogEngine;

        public Agent(string apiKey)
        {
            _dialogEngine = new DialogEngine(apiKey);
        }
        public List<string> Transcript { get; init; } = new List<string>();
        public List<string> Context { get; init; } = new List<string>();

        public async Task<string> RunAsync(string userInput)
        {
            Transcript.Add(userInput);
            await _dialogEngine.RunAsync(Transcript, string.Join('\n', Context));
            return Transcript.Last();
        }
    }
}
