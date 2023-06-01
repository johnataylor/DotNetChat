namespace SysConApp
{
    public class Agent
    {
        private readonly DialogEngine _dialogEngine;
        private readonly Func<string, Task<string>> _fetch;

        public Agent(string apiKey, Func<string, Task<string>> fetch)
        {
            _dialogEngine = new DialogEngine(apiKey);
            _fetch = fetch;
        }

        public bool Verbose
        {
            get => _dialogEngine.Verbose;
            set => _dialogEngine.Verbose = value;
        }

        public List<string> Transcript { get; init; } = new List<string>();
        public List<string> Context { get; init; } = new List<string>();

        public async Task<string> RunAsync(string userInput)
        {
            var assistantResponse = await InnerRunAsync(userInput);

            if (assistantResponse.Contains("NO DATA"))
            {
                // (1) the question couldn't be answered so attempt to access the external data source

                var content = await _fetch(userInput);

                // (2) add the new data to the context (this will end out in the system message)

                Context.Add(content);

                // (3) clean the failed exchanged from our transcript

                TranscriptRemoveLast();
                TranscriptRemoveLast();

                // (4) and have another go at answering the question (remember this time we have the external content)

                assistantResponse = await InnerRunAsync(userInput);

                if (assistantResponse.Contains("NO DATA"))
                {
                    // (5) if we still can't answer the question we are done, inform the user

                    assistantResponse = "I'm sorry but I'm unable to answer your question.";
                    TranscriptRemoveLast();
                    Transcript.Add(assistantResponse);
                }
                else
                {
                    // (6) success, now we have the answer in the transcript, we can clean it out of the context

                    ContextRemoveLast();
                }
            }

            return assistantResponse;
        }

        private async Task<string> InnerRunAsync(string userInput)
        {
            Transcript.Add(userInput);
            await _dialogEngine.RunAsync(Transcript, string.Join('\n', Context));
            return Transcript.Last();
        }

        public void TranscriptRemoveLast()
        {
            Transcript.RemoveAt(Transcript.Count - 1);
        }
        public void ContextRemoveLast()
        {
            Context.RemoveAt(Context.Count - 1);
        }
    }
}
