using Azure.Core;

namespace SysConApp
{
    public class Agent
    {
        private readonly DialogEngine _dialogEngine;
        private readonly Func<string, AccessToken, bool, Task<string>> _structuredFetch;
        private readonly Func<string, AccessToken, bool, Task<string>> _unstructuredFetch;
        private readonly AccessToken _accessToken;

        public Agent(
            string apiKey,
            Func<string, AccessToken, bool, Task<string>> structuredFetch,
            Func<string, AccessToken, bool, Task<string>> unstructuredFetch,
            AccessToken accessToken)
        {
            _dialogEngine = new DialogEngine(apiKey);
            _structuredFetch = structuredFetch;
            _unstructuredFetch = unstructuredFetch;
            _accessToken = accessToken;
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

                if (Verbose)
                {
                    ConsoleLogger.LogQuery(userInput);
                }

                var result = await _structuredFetch(userInput, _accessToken, Verbose);

                if (Verbose)
                {
                    ConsoleLogger.LogQuery(result);
                }

                // (2) add the new data to the context (this will end out in the system message)

                Context.Add(result);

                // (3) clean the failed exchange from our transcript

                TranscriptRemoveLast();
                TranscriptRemoveLast();

                // (4) and have another go at answering the question (remember this time we have the external content)

                assistantResponse = await InnerRunAsync(userInput);

                if (assistantResponse.Contains("NO DATA"))
                {
                    // (5) if we still can't answer the question with the structured endpoint try the unstructured

                    if (Verbose)
                    {
                        ConsoleLogger.LogQuery(userInput);
                    }

                    var unstructuredResult = await _unstructuredFetch(userInput, _accessToken, Verbose);

                    if (Verbose)
                    {
                        ConsoleLogger.LogQueryResult(unstructuredResult);
                    }

                    // (6) add the new data to the context (this will end out in the system message)

                    Context.Add(unstructuredResult);

                    // (7) clean the failed exchange from our transcript

                    TranscriptRemoveLast();
                    TranscriptRemoveLast();

                    // (8) and have another go at answering the question (remember this time we have the external content)

                    assistantResponse = await InnerRunAsync(userInput);

                    if (assistantResponse.Contains("NO DATA"))
                    {
                        assistantResponse = "I'm sorry but I'm unable to answer your question.";
                        TranscriptRemoveLast();
                        Transcript.Add(assistantResponse);
                    }
                    else
                    {
                        // (9) success, now we have the answer in the transcript, we can clean it out of the context

                        ContextRemoveLast();
                    }
                }
                else
                {
                    // (10) success, now we have the answer in the transcript, we can clean it out of the context

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
