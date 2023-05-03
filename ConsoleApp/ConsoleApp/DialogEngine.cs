using Azure.AI.OpenAI;
using System.Text.Json.Nodes;

namespace DotNetChat
{
    public class DialogEngine
    {
        const int MaxAttempts = 10;
        private readonly IDictionary<string, Tool> _tools;
        private readonly MessageFactory _messageFactory;
        private readonly string _apiKey;

        public DialogEngine(List<Tool> tools, MessageFactory messageFactory, string apiKey)
        {
            _tools = tools.ToDictionary(tool => tool.Name);
            _messageFactory = messageFactory;
            _apiKey = apiKey;
        }

        public async Task RunAsync(List<string> transcript)
        {
            var messages = _messageFactory.CreateMessagesFollowingHumanInput(transcript, _tools.Values);

            for(var i=0; i<MaxAttempts; i++)
            {
                var llmResponse = await RunLlmAsync(messages);

                ConsoleLogger.LogLlmResponse(llmResponse.ToJson());

                if (DoWeHaveTheFinalAnswer(llmResponse))
                {
                    ConsoleLogger.LogFinalAnswer();

                    AddToTranscript(transcript, llmResponse.ActionInput);
                    return;
                }
                else if (ShouldWeExecuteATool(llmResponse))
                {
                    var toolResponse = await ExecuteToolAsync(llmResponse);

                    ConsoleLogger.LogToolResponse(toolResponse.Item1);

                    if (toolResponse.Item2)
                    {
                        // return direct
                        AddToTranscript(transcript, toolResponse.Item1);
                        return;
                    }
                    else
                    {
                        messages = _messageFactory.CreateMessagesFollowingToolExecution(transcript, _tools.Values, llmResponse.ToJson(), toolResponse.Item1);
                    }
                }
                else
                {
                    throw new Exception($"tool '{llmResponse.Action}' was not recognized.");
                }
            }

            throw new Exception($"reached max attempts");
        }

        private async Task<LlmResponse> RunLlmAsync(List<ChatMessage> messages)
        {
            var openAIClient = new OpenAIClient(_apiKey, new OpenAIClientOptions());

            var chatCompletionOptions = new ChatCompletionsOptions { StopSequences = { "\nObservation:", "\n\tObservation:" } };

            foreach (var message in messages)
            {
                chatCompletionOptions.Messages.Add(message);
            }

            ConsoleLogger.LogPrompt(chatCompletionOptions.Messages);

            var response = await openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionOptions);

            return new LlmResponse(response.Value.Choices[0].Message.Content);
        }

        private async Task<(string, bool)> ExecuteToolAsync(LlmResponse llmResponse)
        {
            var tool = _tools[llmResponse.Action];

            return (await tool.Function(llmResponse.ActionInput), tool.ReturnDirect);
        }

        private bool DoWeHaveTheFinalAnswer(LlmResponse llmResponse) => llmResponse.Action == "Final Answer";

        private bool ShouldWeExecuteATool(LlmResponse llmResponse) => _tools.ContainsKey(llmResponse.Action);

        private void AddToTranscript(List<string> transcript, string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                transcript.Add(response);
            }
        }

        private class LlmResponse
        {
            public string Action { get; init; }
            public string ActionInput { get; init; }

            public LlmResponse(string content)
            {
                try
                {
                    content = TrimJsonResponse(content);
                    var obj = JsonNode.Parse(content);
                    Action = obj?["action"]?.ToString() ?? throw new Exception("expected 'action' in response");
                    ActionInput = obj?["action_input"]?.ToString() ?? throw new Exception("expected 'action' in response");
                }
                catch (Exception e)
                {
                    throw new Exception(content, e);
                }
            }

            public string ToJson() => $"{{\n  \"action\": \"{Action}\",\n  \"action_input\": \"{ActionInput}\"\n}}";

            private static string TrimJsonResponse(string s)
            {
                var start = s.IndexOf("{");
                var end = s.LastIndexOf("}");
                return s.Substring(start, end - start + 1);
            }
        }
    }
}
